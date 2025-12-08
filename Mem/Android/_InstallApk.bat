del .\MiniCraft.apks

java -jar C:\Libs\bundletool_all_1_8_0.jar build-apks --connected-device --bundle=.\MiniCraft.aab --output .\MiniCraft.apks --ks=.\keyMoonCraft.keystore --ks-pass=pass:moonCrafty@2021 --ks-key-alias=moon_craft_al --key-pass=pass:moonCrafty@2021

java -jar C:\Libs\bundletool_all_1_8_0.jar install-apks --apks=.\MiniCraft.apks

pause