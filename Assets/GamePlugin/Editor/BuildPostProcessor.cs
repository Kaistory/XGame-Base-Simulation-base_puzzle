#define MAX_USE_MYVIEW
#define MAX_USE_CONSTRAINTS

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Callbacks;
using System.Threading;
#if UNITY_IOS || UNITY_IPHONE
using UnityEditor.iOS.Xcode;
#endif

#if UNITY_IOS || UNITY_IPHONE
public class BuildPostProcessor
{

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {

            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);


            PlistElementDict rootDict = plist.root;

            // example of changing a value:
            // rootDict.SetString("CFBundleVersion", "6.6.6");

            // example of adding a boolean key...
            // < key > ITSAppUsesNonExemptEncryption </ key > < false />
            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-2777953690987264~9641495417");
            rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to deliver personalized ads to you.");

            PlistElementArray adNetworksItems = rootDict["SKAdNetworkItems"] == null
                ? rootDict.CreateArray("SKAdNetworkItems")
                : rootDict["SKAdNetworkItems"].AsArray();
            List<string> existingadNetworksItems = new List<string>();
            foreach (PlistElement element in adNetworksItems.values)
            {
                existingadNetworksItems.Add(element.AsDict().values["SKAdNetworkIdentifier"].AsString());
            }
            bool isAddBigo = false;
#if ENABLE_ADS_IRON || ENABLE_ADS_MAX
            if (isAddBigo)
            {
                List<string> dicnet = getDicnetSkadnet(Application.dataPath + "/GamePlugin/Ads/Editor/BigoAds/BigoSKAdNetworkItems.txt");
                foreach (string itemnet in dicnet)
                {
                    if (!existingadNetworksItems.Contains(itemnet))
                    {
                        PlistElementDict adNetworksItem = adNetworksItems.AddDict();
                        adNetworksItem.SetString("SKAdNetworkIdentifier", itemnet);
                        existingadNetworksItems.Add(itemnet);
                    }
                }
            }
#endif
#if ENABLE_ADS_MYTARGET
            List<string> dicnettarget = new List<string>() { "r26jy69rpl.skadnetwork" };
            foreach (string itemnet in dicnettarget)
            {
                if (!existingadNetworksItems.Contains(itemnet))
                {
                    PlistElementDict adNetworksItem = adNetworksItems.AddDict();
                    adNetworksItem.SetString("SKAdNetworkIdentifier", itemnet);
                    existingadNetworksItems.Add(itemnet);
                }
            }
#endif
#if ENABLE_ADS_TOPON && !USE_TOPONMY
            List<string> dicnet = getDicnetSkadnet(Application.dataPath + "/GamePlugin/Ads/Topon/Editor/SKAdNetworkItems.txt");
            foreach (string itemnet in dicnet)
            {
                if (!existingadNetworksItems.Contains(itemnet))
                {
                    PlistElementDict adNetworksItem = adNetworksItems.AddDict();
                    adNetworksItem.SetString("SKAdNetworkIdentifier", itemnet);
                    existingadNetworksItems.Add(itemnet);
                }
            }
            PlistElementArray arrSchecms = rootDict["LSApplicationQueriesSchemes"] == null
                ? rootDict.CreateArray("LSApplicationQueriesSchemes")
                : rootDict["LSApplicationQueriesSchemes"].AsArray();
            List<string> existingSchemes = new List<string>();
            foreach (PlistElement element in arrSchecms.values)
            {
                existingSchemes.Add(element.AsString());
            }
            List<string> dicSchemes = getAppQueriesSchemesTopon();
            foreach (string itemnet in dicSchemes)
            {
                if (!existingSchemes.Contains(itemnet))
                {
                    arrSchecms.AddString(itemnet);
                    existingSchemes.Add(itemnet);
                }
            }
#endif

            string[] arrNetAdd = {
                "v9wttpbfk9.skadnetwork",
                "n38lu8286q.skadnetwork"
            };
            foreach (string itemnet in arrNetAdd)
            {
                if (!existingadNetworksItems.Contains(itemnet))
                {
                    PlistElementDict adNetworksItem = adNetworksItems.AddDict();
                    adNetworksItem.SetString("SKAdNetworkIdentifier", itemnet);
                    existingadNetworksItems.Add(itemnet);
                }
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }

    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            // We need to construct our own PBX project path that corrently refers to the Bridging header
            // var projPath = PBXProject.GetPBXProjectPath(buildPath);
            var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            //var targetGuid = proj.GetUnityFrameworkTargetGuid();
            var targetGuid = proj.GetUnityMainTargetGuid();
            var gameAssemblyTargetGuid = proj.TargetGuidByName("GameAssembly");


            //// Configure build settings
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            //proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/MyUnityPlugin/Source/MyUnityPlugin-Bridging-Header.h");
            //proj.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "MyUnityPlugin-Swift.h");
            //proj.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

            var frameworkGuid = proj.GetUnityFrameworkTargetGuid();
            proj.SetBuildProperty(frameworkGuid, "ENABLE_BITCODE", "NO");
            proj.SetBuildProperty(frameworkGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            proj.WriteToFile(projPath);

            addNativeViewXib(projPath, buildPath);
            RemoveDuplicateRunScriptGUIDsFromProjectFile(projPath, gameAssemblyTargetGuid);
            try
            {
                if (AppConfig.AdjustBanner > 0)
                {
#if ENABLE_ADS_MAX || ENABLE_ADS_IRON
                    fixBannerMax(buildPath, true);
                    fixBannetIron(buildPath);
#endif
                    fixBannerAdmob(buildPath);
                    MyTargetPostProcess.AdjustBanner(buildPath);
                    YandexPostProcess.AdjustBanner(buildPath);
                }
            }
            catch (Exception ex) { }
        }
    }

    static void addNativeViewXib(string projPath, string buildPath)
    {
        // Danh sách đầy đủ file bạn cần thêm và đường dẫn tương đối trong Xcode project
        List<(string Key, string Value)> resources = new List<(string, string)>
        {
            ("GGNativeAdCl.xib", "Libraries/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/GGNativeAdCl.xib"),
            ("GGNativeAdBn.xib", "Libraries/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/GGNativeAdBn.xib"),
            ("GGNativeAdRectNt.xib", "Libraries/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/GGNativeAdRectNt.xib"),
            ("GGNativeAdView.xib", "Libraries/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/GGNativeAdView.xib"),
            ("MaxNativeAdView.xib", "Libraries/GamePlugin/Ads/MaxPlugin/Platforms/iOS/MaxNativeAdView.xib"),
            ("FbNativeAdView.xib", "Libraries/GamePlugin/Ads/FbPlugin/Platforms/iOS/FbNativeAdView.xib"),
            ("button_close.png", "Libraries/GamePlugin/Plugins/iOS/button_close.png"),
            ("icon_down.png", "Libraries/GamePlugin/Plugins/iOS/icon_down.png"),
            ("button_close.png", "Libraries/button_close.png"),
            ("icon_down.png", "Libraries/icon_down.png")
        };

        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        string projText = File.ReadAllText(projPath); // đọc để kiểm tra thủ công các file đã nằm trong Resources
        string targetGuid = proj.GetUnityMainTargetGuid();
        string frameworkGuid = proj.GetUnityFrameworkTargetGuid();
        string resourcesBuildPhaseGuid = proj.GetResourcesBuildPhaseByTarget(targetGuid);

        // Đảm bảo tắt Bitcode và Swift Embed
        proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(frameworkGuid, "ENABLE_BITCODE", "NO");
        proj.SetBuildProperty(frameworkGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        foreach (var kvp in resources)
        {
            string fileName = kvp.Key;
            string localPath = kvp.Value.Replace("\\", "/");
            string fullPath = Path.Combine(buildPath, localPath);

            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"❌ File not found: {fullPath}");
                continue;
            }

            bool alreadyInProject = proj.ContainsFileByProjectPath(localPath);

            string fileGuid = alreadyInProject
                ? proj.FindFileGuidByProjectPath(localPath)
                : proj.AddFile(localPath, localPath, PBXSourceTree.Source);

            if (!IsFileInMainResourcesPhase(projText, resourcesBuildPhaseGuid, fileName))
            {
                proj.AddFileToBuildSection(targetGuid, resourcesBuildPhaseGuid, fileGuid);
                Debug.Log($"✅ Added to Copy Bundle Resources: {fileName}");
            }
            else
            {
                Debug.Log($"ℹ️ File already in Copy Bundle Resources: {fileName}");
            }
        }

        proj.WriteToFile(projPath);
        Debug.Log("✅ Completed updating Xcode project with resource files.");
    }

    static void RemoveDuplicateRunScriptGUIDsFromProjectFile(string projPath, string targetGuid)
    {
        string[] lines = File.ReadAllLines(projPath);
        List<string> output = new List<string>();

        bool insideTarget = false;
        bool insideBuildPhases = false;
        HashSet<string> scriptGuids = new HashSet<string>();
        HashSet<string> duplicatesToRemove = new HashSet<string>();

        foreach (string line in lines)
        {
            string trimmed = line.Trim();

            if (trimmed.StartsWith($"{targetGuid} ") && trimmed.Contains("/* GameAssembly */"))
            {
                insideTarget = true;
            }

            if (insideTarget && trimmed.StartsWith("buildPhases = ("))
            {
                insideBuildPhases = true;
                output.Add(line);
                continue;
            }

            if (insideBuildPhases)
            {
                if (trimmed == ");")
                {
                    insideBuildPhases = false;
                    insideTarget = false;
                    output.Add(line);
                    continue;
                }

                if (trimmed.Contains("/* Run Script */"))
                {
                    string guid = trimmed.Split(' ')[0].Trim();
                    if (!scriptGuids.Add(guid))
                    {
                        duplicatesToRemove.Add(guid);
                        Debug.Log($"🗑 Found duplicate Run Script GUID: {guid}");
                        continue; // skip adding this duplicate line
                    }
                }
                
                if (trimmed.Contains("/* ShellScript */"))
                {
                    string guid = trimmed.Split(' ')[0].Trim();
                    if (!scriptGuids.Add(guid))
                    {
                        duplicatesToRemove.Add(guid);
                        Debug.Log($"🗑 Found duplicate Run Script GUID: {guid}");
                        continue; // skip adding this duplicate line
                    }
                }
            }

            output.Add(line);
        }

        File.WriteAllLines(projPath, output.ToArray());
        Debug.Log($"✅ Cleaned project.pbxproj. Removed {duplicatesToRemove.Count} duplicate Run Script entries.");
    }

    static bool IsFileInMainResourcesPhase(string pbxText, string resourcesPhaseGuid, string fileName)
    {
        string pattern = $@"{Regex.Escape(resourcesPhaseGuid)}.*?files\s*=\s*\(([\s\S]*?)\);";
        Regex regex = new Regex(pattern, RegexOptions.Singleline);
        Match match = regex.Match(pbxText);

        if (!match.Success)
        {
            Debug.LogWarning($"[XcodePostProcess] Resources Build Phase GUID not found: {resourcesPhaseGuid}");
            return false;
        }

        string content = match.Groups[1].Value;
        string filePattern = $@"\/\*\s*{Regex.Escape(fileName)}\s*in\s*Resources\s*\*\/";
        bool found = Regex.IsMatch(content, filePattern);

        Debug.Log($"[XcodePostProcess] Checking file '{fileName}' in Resources phase: {(found ? "FOUND" : "NOT FOUND")}");
        return found;
    }

    public static void fixBannerAdmob(string path)
    {
        string pathsrc = Application.dataPath + "/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/AdmobBanner.m";
        string pfilem = path + "/Libraries/GamePlugin/Ads/AdmobPlugin/Platforms/iOS/AdmobBanner.m";
        SettingBuildAndroid.coppyFileNormal(pathsrc, pfilem);
        if (File.Exists(pfilem))
        {
            string[] linesRead = File.ReadAllLines(pfilem);
            bool isw = false;
            for (int i = 0; i < linesRead.Length; i++)
            {
                if (linesRead[i].Contains("unityView.safeAreaInsets.top / 1;"))
                {
                    isw = true;
                    linesRead[i] = linesRead[i].Replace("unityView.safeAreaInsets.top / 1;", $"unityView.safeAreaInsets.top / {AppConfig.AdjustBanner};");
                }
                else if (linesRead[i].Contains("unityView.safeAreaInsets.bottom / 1;"))
                {
                    isw = true;
                    linesRead[i] = linesRead[i].Replace("unityView.safeAreaInsets.bottom / 1;", $"unityView.safeAreaInsets.bottom / {AppConfig.AdjustBanner};");
                }
            }
            if (isw)
            {
                System.IO.File.WriteAllLines(pfilem, linesRead);
            }
        }
    }

    static void fixBannetIron(string path)
    {
        string pfilem = path + "/Libraries/LevelPlay/Runtime/Plugins/iOS/iOSBridge.m";
        if (File.Exists(pfilem))
        {
            string[] linesRead = File.ReadAllLines(pfilem);
            List<string> linesWrite = new List<string>();
            int stateCheck = 0;
            bool isw = false;
            for (int i = 0; i < linesRead.Length; i++)
            {
                if (stateCheck == 0)
                {
                    if (linesRead[i].Contains("- (CGPoint)getBannerCenter:(NSInteger)position rootView:(UIView *)rootView {"))
                    {
                        stateCheck = 1;
                    }
                    linesWrite.Add(linesRead[i]);
                }
                else if (stateCheck == 1)
                {
                    if (linesRead[i].Contains("+ rootView.safeAreaInsets.top;"))
                    {
                        string newline = linesRead[i].Replace("+ rootView.safeAreaInsets.top;", $"+ rootView.safeAreaInsets.top / {AppConfig.AdjustBanner};");
                        linesWrite.Add(newline);
                        isw = true;
                    }
                    else if (linesRead[i].Contains("- rootView.safeAreaInsets.bottom;"))
                    {
                        string newline = linesRead[i].Replace("- rootView.safeAreaInsets.bottom;", $"- rootView.safeAreaInsets.bottom / {AppConfig.AdjustBanner};");
                        linesWrite.Add(newline);
                        isw = true;
                    }
                    else if (linesRead[i].Contains("return CGPointMake(rootView.frame.size.width / 2, y);"))
                    {
                        stateCheck = 2;
                        linesWrite.Add(linesRead[i]);
                    }
                    else
                    {
                        linesWrite.Add(linesRead[i]);
                    }
                }
                else
                {
                    linesWrite.Add(linesRead[i]);
                }
            }
            if (isw)
            {
                System.IO.File.WriteAllLines(pfilem, linesWrite);
                linesWrite.Clear();
            }
        }
    }

    static void fixBannerMax(string path, bool isPortrait = false)
    {
        string pfilem = path + "/Libraries/MaxSdk/AppLovin/Plugins/iOS/MAUnityAdManager.m";
        if (File.Exists(pfilem))
        {
            string[] linesRead = File.ReadAllLines(pfilem);
            List<string> linesWrite = new List<string>();
            int stateCheck = 0;
            bool isw = false;
            bool isComment = false;
            int logic4hide = 0;
            string scm = "//";
            string sncm = "";
            for (int i = 0; i < linesRead.Length; i++)
            {
                if (linesRead[i].Contains("-(void)setPosBanner4Show:(UIView*)bannerView adPos:(NSString *)adViewPosition bannerWidth:(float)width bannerHeight:(float)bnh"))
                {
                    return;
                }
            }

#if MAX_USE_MYVIEW
            scm = "";
            sncm = "//";
#endif
            for (int i = 0; i < linesRead.Length; i++)
            {
                if (stateCheck == 0)
                {
                    linesWrite.Add(linesRead[i]);
                    if (linesRead[i].Contains("@property (nonatomic, strong) NSObject *adInfoDictLock;"))
                    {
                        linesWrite.Add("@property (nonatomic, strong) UIView *bannerParent;");
                        linesWrite.Add("@property (nonatomic) BOOL isIPad;");
                    }
                    else if (linesRead[i].Contains("[rootViewController.view addSubview: self.safeAreaBackground];"))
                    {
                        if (linesRead[i - 3].Contains("self.safeAreaBackground.userInteractionEnabled = NO;"))
                        {
                            isw = true;
                            linesWrite.Add("            if ( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )");
                            linesWrite.Add("            {");
                            linesWrite.Add("                            self.isIPad = YES;");
                            linesWrite.Add("            }");
                        }
                    }
                    else if (linesRead[i].Contains("// Check edge case where ad may be detatched from view controller"))
                    {
                        bool idaddbl = false;
                        for (int mn = 1; mn < 5; mn++)
                        {
                            if (linesRead[i + mn].Contains("!view.window.rootViewController"))
                            {
                                idaddbl = true;
                                break;
                            }
                        }
                        if (idaddbl)
                        {
                            isw = true;
                            linesWrite.Add("            CGFloat adViewWidth = self.adViewWidths[adUnitIdentifier].floatValue;");
                            if (isPortrait)
                            {
                                linesWrite.Add("            if (adViewWidth > -10) {");
                            }
                            else
                            {
                                linesWrite.Add("            if (adViewWidth > 700) {");
                            }

                            linesWrite.Add("                if (!view.window.rootViewController)");
                            linesWrite.Add("                {");
                            linesWrite.Add("                    [self log: @\" %@ missing view controller - re - attaching to %@...\", adFormat, [self unityViewController]];");
                            linesWrite.Add($"                    {scm}[self creatParentBanner:adViewWidth bnView:view adUnitId:adUnitIdentifier];");
                            linesWrite.Add($"                    {scm}if (view.superview != self.bannerParent) " + "{");
                            linesWrite.Add($"                    {scm}    [view removeFromSuperview];");
                            linesWrite.Add($"                    {scm}    [self.bannerParent addSubview: view];");
                            linesWrite.Add($"                    {scm}" + "}");
                            linesWrite.Add($"                    {sncm}UIViewController *rootViewController = [self unityViewController];");
                            linesWrite.Add($"                    {sncm}[rootViewController.view addSubview: view];");
                            linesWrite.Add($"                    {sncm}[self positionAdViewForAdUnitIdentifier: adUnitIdentifier adFormat: adFormat];");
                            linesWrite.Add("                }");
                            linesWrite.Add("            } else {");
                            linesWrite.Add("                [self creatParentBanner:adViewWidth bnView:view adUnitId:adUnitIdentifier];");
                            linesWrite.Add("                if (view.superview != self.bannerParent) {");
                            linesWrite.Add("                    [view removeFromSuperview];");
                            linesWrite.Add("                    [self.bannerParent addSubview: view];");
                            linesWrite.Add("                    [self positionAdViewForAdUnitIdentifier: adUnitIdentifier adFormat: adFormat];");
                            linesWrite.Add("                }");
                            linesWrite.Add("            }");
                            linesWrite.Add("        }");
                            for (int mn = 0; mn < 15; mn++)
                            {
                                i++;
                                if (linesRead[i].Contains("        self.safeAreaBackground.hidden = NO;"))
                                {
                                    linesWrite.Add(linesRead[i]);
#if MAX_USE_MYVIEW
                                    if (!linesRead[i + 1].Contains("self.bannerParent.hidden = NO"))
                                    {
                                        linesWrite.Add("        self.bannerParent.hidden = NO;");
                                    }
#endif
                                    break;
                                }
                            }
                        }
                    }
                    else if (linesRead[i].Contains("- (MAAdView *)retrieveAdViewForAdUnitIdentifier:(NSString *)adUnitIdentifier adFormat:(MAAdFormat *)adFormat atPosition:(NSString *)adViewPosition withOffset:(CGPoint)offset"))
                    {
                        int countadd = 0;
                        for (int mn = 1; mn < 25; mn++)
                        {
                            countadd++;
                            linesWrite.Add(linesRead[i + mn]);
                            if (linesRead[i + mn].Contains("self.adViewOffsets[adUnitIdentifier] = [NSValue valueWithCGPoint: offset];"))
                            {
                                break;
                            }
                        }
                        if (countadd > 5)
                        {
                            isw = true;
                            linesWrite.Add("");
                            linesWrite.Add("        CGFloat adViewWidth = self.adViewWidths[adUnitIdentifier].floatValue;");
                            if (isPortrait)
                            {
                                linesWrite.Add("        if (adViewWidth > -10) {");
                            }
                            else
                            {
                                linesWrite.Add("        if (adViewWidth > 700) {");
                            }
                            linesWrite.Add($"            {sncm}UIViewController *rootViewController = [self unityViewController];");
                            linesWrite.Add($"            {sncm}[rootViewController.view addSubview: result];");
                            linesWrite.Add($"            {scm}[self creatParentBanner:adViewWidth bnView:result adUnitId:adUnitIdentifier];");
                            linesWrite.Add($"            {scm}[result removeFromSuperview];");
                            linesWrite.Add($"            {scm}[self.bannerParent addSubview: result];");
                            linesWrite.Add("        } else {");
                            linesWrite.Add("            [self creatParentBanner:adViewWidth bnView:result adUnitId:adUnitIdentifier];");
                            linesWrite.Add("            [result removeFromSuperview];");
                            linesWrite.Add("            [self.bannerParent addSubview: result];");
                            linesWrite.Add("        }");
                            linesWrite.Add("");

                            for (int mn = 1; mn < 25; mn++)
                            {
                                i++;
                                if (linesRead[i].Contains("// Allow pubs to pause auto-refresh immediately, by default."))
                                {
                                    linesWrite.Add(linesRead[i]);
                                    break;
                                }
                            }
                        }
                    }
                    else if (logic4hide == 0 && linesRead[i].Contains("- (void)hideAdViewWithAdUnitIdentifier:(NSString *)adUnitIdentifier adFormat:(MAAdFormat *)adFormat"))
                    {
                        logic4hide = 1;
                    }
                    else if (logic4hide > 0 && linesRead[i].Contains("view.hidden = YES;"))
                    {
                        logic4hide = 0;
#if MAX_USE_MYVIEW
                        linesWrite.Add("        self.bannerParent.hidden = YES;");
#endif
                    }
                    if (linesRead[i].Contains("- (void)positionAdViewForAdUnitIdentifier:(NSString *)adUnitIdentifier adFormat:(MAAdFormat *)adFormat"))
                    {
                        stateCheck = 1;
                    }
                }
                else if (stateCheck == 1)
                {
                    if (!isComment)
                    {
                        if (linesRead[i].Contains("NSArray<NSLayoutConstraint *> *activeConstraints = self.adViewConstraints[adUnitIdentifier];")
                            || linesRead[i].Contains("CGSize adViewSize = CGSizeMake(adViewWidth, adViewHeight);")
                            )
                        {
                            isComment = false;
#if MAX_USE_MYVIEW && !MAX_USE_CONSTRAINTS
                            isComment = true;
#endif
                        }
                    }
                    else
                    {
                        if (linesRead[i].Contains("CGFloat adViewWidth;"))
                        {
                            isComment = false;
                        }
                    }

                    if (linesRead[i].Contains("[NSLayoutConstraint activateConstraints: constraints];"))
                    {
#if MAX_USE_MYVIEW && !MAX_USE_CONSTRAINTS
                        linesWrite.Add("//" + linesRead[i]);
#else
                        linesWrite.Add(linesRead[i]);
#endif
                        if (!linesRead[i + 1].Contains("[self setPosBanner4Show:adView adPos:adViewPosition bannerWidth:adViewWidth bannerHeight:adViewHeight];"))
                        {
                            linesWrite.Add("        [self setPosBanner4Show:adView adPos:adViewPosition bannerWidth:adViewWidth bannerHeight:adViewHeight];");
                            isw = true;
                        }
                        stateCheck = 2;
                    }
                    else if (linesRead[i].Contains("[constraints addObject: [adView.bottomAnchor constraintEqualToAnchor: layoutGuide.bottomAnchor"))
                    {
#if MAX_USE_MYVIEW && !MAX_USE_CONSTRAINTS
                        linesWrite.Add($"//                    [constraints addObject: [adView.bottomAnchor constraintEqualToAnchor: layoutGuide.bottomAnchor constant:superview.safeAreaInsets.bottom / {AppConfig.AdjustBanner}]];");
#else
                        linesWrite.Add($"                    [constraints addObject: [adView.bottomAnchor constraintEqualToAnchor: layoutGuide.bottomAnchor constant:superview.safeAreaInsets.bottom / {AppConfig.AdjustBanner}]];");
#endif
                    }
                    else
                    {
                        if (!isComment)
                        {
                            linesWrite.Add(linesRead[i]);
                        }
                        else
                        {
                            linesWrite.Add("//" + linesRead[i]);
                        }
                    }
                }
                else if (stateCheck == 2)
                {
                    linesWrite.Add(linesRead[i]);
                    if (linesRead[i].CompareTo("}") == 0)
                    {
                        bool isadd = false;
                        for (int j = 0; j < 4; j++)
                        {
                            if (linesRead[i].Contains("-(void)setPosBanner4Show:(UIView*)bn adPos:(NSString *)adViewPosition bannerWidth:(float)bnw bannerHeight:(float)bnh"))
                            {
                                isadd = true;
                            }
                        }
                        if (!isadd)
                        {
                            isw = true;
                            linesWrite.Add("");
                            AddCodeFix(linesWrite, isPortrait);
                        }
                        stateCheck = -1;
                    }
                }
                else
                {
                    linesWrite.Add(linesRead[i]);
                }
            }
            if (isw)
            {
                System.IO.File.WriteAllLines(pfilem, linesWrite);
                linesWrite.Clear();
            }
        }
    }

    static void AddCodeFix(List<string> linesWrite, bool isPortrait = false)
    {
        string[] all = {
            "-(void)setPosBanner4Show:(UIView*)bannerView adPos:(NSString *)adViewPosition bannerWidth:(float)width bannerHeight:(float)bnh",
            "{",
            "    if (width > 700) {",
            "        UIView* unityView = [self unityViewController].view;",
            "        float wscr = unityView.bounds.size.width;",
            "        if (wscr < unityView.bounds.size.height)",
            "        {",
            "            //wscr = unityView.bounds.size.height;",
            "        }",
            "        float xbn = (wscr - width) / 2;",
            "        CGFloat ybn = 0;",
            "        if ([adViewPosition containsString: @\"bottom_\"])",
            "        {",
            "            ybn = unityView.bounds.size.height - bnh;",
            "        }",
            "        float safebo = unityView.safeAreaInsets.bottom;",
            "        if (safebo > 0 && ybn > 0)",
            "        {",
            $"            ybn -= safebo / {AppConfig.AdjustBanner};",
            "        }",
            "        bannerView.frame = CGRectMake(0, 0, width, bnh);",
            "        if (self.bannerParent != nil && [adViewPosition containsString: @\"top_\"])",
            "        {",
            "            self.bannerParent.frame = CGRectMake(xbn, 0, width, bannerView.bounds.size.height);",
            "            [self.bannerParent setNeedsDisplay];",
            "            [self.bannerParent setNeedsLayout];",
            "            NSLog(@\"mysdk: admax banner setPosBanner4Show xbn=%f ybn=%f wbn=%f\", xbn, ybn, width);",
            "        }",
            "        else if (self.bannerParent != nil)",
            "        {",
            "            self.bannerParent.frame = CGRectMake(xbn, ybn, width, bannerView.bounds.size.height);",
            "            [self.bannerParent setNeedsDisplay];",
            "            [self.bannerParent setNeedsLayout];",
            "            NSLog(@\"mysdk: admax banner setPosBanner4Show xbn=%f ybn=%f wbn=%f\", xbn, ybn, width);",
            "        }",
            "    }",
            "    else",
            "    {",
            "        UIView* unityView = [self unityViewController].view;",
            "        float wscr = unityView.bounds.size.width;",
            "        int rwidth = width;",
            "        int bnw = bannerView.bounds.size.width;",
            "        if (width <= 10)",
            "        {",
            "            rwidth = bnw;",
            "        }",
            $"        float dxCenter = {AppConfig.DxCenter4Max};",
            "        float xbn = (wscr - rwidth) / 2 + dxCenter * wscr;",
            "        bannerView.frame = CGRectMake(0, 0, rwidth, bannerView.bounds.size.height);",
            "        if (self.bannerParent != nil && [adViewPosition containsString: @\"top_\"])",
            "        {",
            "            self.bannerParent.frame = CGRectMake(xbn, 0, rwidth, bannerView.bounds.size.height);",
            "            [self.bannerParent setNeedsDisplay];",
            "            [self.bannerParent setNeedsLayout];",
            "            NSLog(@\"mysdk: admax banner setPosBanner4Show xbn=%f ybn=0 rwidth=%d\", xbn, rwidth);",
            "        }",
            "        else if (self.bannerParent != nil)",
            "        {",
            "            float ybn = unityView.bounds.size.height - bannerView.bounds.size.height;",
            "            self.bannerParent.frame = CGRectMake(xbn, ybn, rwidth, bannerView.bounds.size.height);",
            "            [self.bannerParent setNeedsDisplay];",
            "            [self.bannerParent setNeedsLayout];",
            "            NSLog(@\"mysdk: admax banner setPosBanner4Show xbn =% f ybn =% f rwidth =% d\", xbn, ybn, rwidth);",
            "        }",
            "    }",
            "    [bannerView setNeedsDisplay];",
            "    [bannerView setNeedsLayout];",
        "}",
            "",
            "- (void) creatParentBanner: (int) viewWidth bnView:(UIView*) bnView adUnitId:(NSString*) adUnitId",
            "{",
            "    if (self.bannerParent == nil) {",
            "        int rwidth = viewWidth;",
            "        int bnw = bnView.bounds.size.width;",
            "        if (viewWidth <= 10) {",
            "            rwidth = bnw;",
            "        }",
            "        UIViewController* vcon = [self unityViewController];",
            $"        float dxCenter = {AppConfig.DxCenter4Max};",
            "        float xbn = (vcon.view.bounds.size.width - rwidth) / 2 + dxCenter * vcon.view.bounds.size.width;",
            "        NSString* adViewPosition = self.adViewPositions[adUnitId];",
            "        if ([adViewPosition containsString: @\"top_\"])",
            "        {",
            "            self.bannerParent = [[UIView alloc] initWithFrame: CGRectMake(xbn, 0, rwidth, bnView.bounds.size.height)];",
            "        }",
            "        else",
            "        {",
            "            self.bannerParent = [[UIView alloc] initWithFrame: CGRectMake(xbn, vcon.view.bounds.size.height - bnView.bounds.size.height, rwidth, bnView.bounds.size.height)];",
            "        }",
            "        // self.bannerParent.backgroundColor = [UIColor redColor];",
            "        //self.bannerParent.userInteractionEnabled = NO;",
            "        self.bannerParent.clipsToBounds = true;",
            "        [vcon.view addSubview:self.bannerParent];",
            "    }",
            "}",
        };
        if (isPortrait)
        {
            all[2] = "    if (width > -10) {";
            all[7] = "            //wscr = unityView.bounds.size.height;";
        }
        else
        {

        }
        for (int i = 0; i < all.Length; i++)
        {
            linesWrite.Add(all[i]);
        }
    }

    public static List<string> getDicnetSkadnet(string path)
    {
        List<string> dicnet = new List<string>();
        if (File.Exists(path))
        {
            var appCf = File.ReadAllLines(path);
            for (int i = 0; i < appCf.Length; i++)
            {
                int idxfind = 0;
                while (idxfind < appCf[i].Length)
                {
                    int idxsub = appCf[i].IndexOf("<string>", idxfind);
                    if (idxsub < 0)
                    {
                        break;
                    }
                    else
                    {
                        int idxff = appCf[i].IndexOf('.', idxsub + 8);
                        if (idxff > idxsub)
                        {
                            idxsub += 8;
                            string SKAdNetworkIdentifier = appCf[i].Substring(idxsub, idxff - idxsub);
                            dicnet.Add(SKAdNetworkIdentifier.ToLower() + ".skadnetwork");
                            idxfind = idxff;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
        }

        return dicnet;
    }

    public static List<string> getAppQueriesSchemesTopon()
    {
        List<string> dicnet = new List<string>();

        string passet = Application.dataPath + "//GamePlugin/Ads/Topon/Editor/ApplicationQueriesSchemes.txt";
        if (File.Exists(passet))
        {
            var appCf = File.ReadAllLines(passet);
            for (int i = 0; i < appCf.Length; i++)
            {
                int idxfind = 0;
                while (idxfind < appCf[i].Length)
                {
                    int idxsub = appCf[i].IndexOf("<string>", idxfind);
                    if (idxsub < 0)
                    {
                        break;
                    }
                    else
                    {
                        int idxff = appCf[i].IndexOf("</", idxsub + 8);
                        if (idxff > idxsub)
                        {
                            idxsub += 8;
                            string SKAdNetworkIdentifier = appCf[i].Substring(idxsub, idxff - idxsub);
                            dicnet.Add(SKAdNetworkIdentifier);
                            idxfind = idxff;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
        }

        return dicnet;
    }
}
#endif
