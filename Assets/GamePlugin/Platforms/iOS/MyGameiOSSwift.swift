//
//  TestHaptic.swift
//  UnityFramework
//
//  Created by Nguyen Viet on 9/29/20.
//

import Foundation
import UIKit
import StoreKit
import UserNotifications

@objc public class MyGameiOSSwift : NSObject {
    let GroupIMessageId = "group.com.app.enter.mix.match.emo.sticker"
    @objc public static let shared = MyGameiOSSwift()
    
    @objc public func Vibrate(type:Int) {
        switch type {
        case 1:
            //print("haptic error")
            let generator = UINotificationFeedbackGenerator()
            generator.notificationOccurred(.error)
            
        case 2:
            //print("haptic success")
            let generator = UINotificationFeedbackGenerator()
            generator.notificationOccurred(.success)
            
        case 3:
            //print("haptic warning")
            let generator = UINotificationFeedbackGenerator()
            generator.notificationOccurred(.warning)
            
        case 4:
            //print("haptic impactOccurred light")
            let generator = UIImpactFeedbackGenerator(style: .light)
            generator.impactOccurred()
            
        case 5:
            //print("haptic impactOccurred medium")
            let generator = UIImpactFeedbackGenerator(style: .medium)
            generator.impactOccurred()
            
        case 6:
            //print("haptic impactOccurred heavy")
            let generator = UIImpactFeedbackGenerator(style: .heavy)
            generator.impactOccurred()
            
        default:
            //print("haptic selectionChanged")
            let generator = UISelectionFeedbackGenerator()
            generator.selectionChanged()
        }
    }
    
    @objc public func MyAppReview() -> Bool {
        if #available(iOS 10.3, *) {
            let twoSecondsFromNow = DispatchTime.now() + 0.75
            DispatchQueue.main.asyncAfter(deadline: twoSecondsFromNow) {
                SKStoreReviewController.requestReview()
            }
            return true;
        } else {
            return false;
        }
    }
    
    @objc public func LocalNotify(title:String, msg:String, hour:Int, minute:Int, repeatday:Int) {
        let content = UNMutableNotificationContent()
        content.title = title
        content.body = msg
        content.sound = UNNotificationSound.default
        content.badge = 1;
        
        var dateCompo = DateComponents()
        if (repeatday >= 0) {
            dateCompo.weekday = repeatday
        }
        dateCompo.hour = hour
        dateCompo.minute = minute
        
        let idNoti = String(format: "%d-%02d:%02d", repeatday, hour, minute)
        print(idNoti)
        let trigger = UNCalendarNotificationTrigger.init(dateMatching: dateCompo, repeats: true)
        let request = UNNotificationRequest(identifier: idNoti, content: content, trigger: trigger)
        UNUserNotificationCenter.current().add(request, withCompletionHandler: nil)
    }
    
    @objc public func clearAllNoti() {
        let center = UNUserNotificationCenter.current()
        center.removeAllPendingNotificationRequests()
        center.removeAllDeliveredNotifications()
    }
    
    
    // MARK: Save one sticker to app
    @discardableResult
    @objc public func saveSticker(to group: String, name: String, data: NSData) -> Bool {
        if group.isEmpty || name.isEmpty || data.isEmpty {
            return false
        }
        print("STICKERS: \(group) \(name) \(data.count)")
        guard let storage = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: GroupIMessageId)?.appendingPathComponent("Library/Caches") else {
            return false
        }
        let groupUrl = storage.appendingPathComponent("\(group)")
        var isDir : ObjCBool = true
        if !FileManager.default.fileExists(atPath: groupUrl.path, isDirectory: &isDir) {
            do {
                print("STICKERS: CREATE GROUP \(group) \(name) \(data.count)")
                try FileManager.default.createDirectory(at: groupUrl, withIntermediateDirectories: false, attributes: nil)
            } catch {
                print("STICKERS: CREATE GROUP ERROR \(group) \(name) \(data.count)")
                return false
            }
        }
        let fileUrl = storage.appendingPathComponent("\(group)/\(name).png")
        let jsonUrl = storage.appendingPathComponent("stickers.json")
        if let jsonContent = try? Data(contentsOf: jsonUrl) {
            do {
                print("STICKERS: CREATE STICKER TO EXITS GROUP \(group) \(name) \(data.count)")
                var stickers = try JSONDecoder().decode([StickerModel].self, from: jsonContent)
                if let sticker = stickers.first(where: { model in
                    return model.group == group
                }) {
                    sticker.icons.append(name)
                } else {
                    let sticker = StickerModel()
                    sticker.group = group
                    sticker.icons = [name]
                    stickers.append(sticker)
                }
                try data.write(to: fileUrl)
                if FileManager.default.fileExists(atPath: jsonUrl.path) {
                    try FileManager.default.removeItem(at: jsonUrl)
                }
                let stickersData = try JSONEncoder().encode(stickers)
                try stickersData.write(to: jsonUrl)
                return true
            } catch {
                return false
            }
        } else {
            do {
                print("STICKERS: CREATE STICKER TO GROUP \(group) \(name) \(data.count)")
                var stickers = [StickerModel]()
                let sticker = StickerModel()
                sticker.group = group
                sticker.icons = [name]
                stickers.append(sticker)
                try data.write(to: fileUrl)
                if FileManager.default.fileExists(atPath: jsonUrl.path) {
                    try FileManager.default.removeItem(at: jsonUrl)
                }
                let stickersData = try JSONEncoder().encode(stickers)
                try stickersData.write(to: jsonUrl)
                return true
            } catch {
                return false
            }
        }
    }
    
    // MARK: Delete one sticker to app
    @discardableResult
    @objc public func deleteSticker(from group: String, name: String) -> Bool {
        if group.isEmpty || name.isEmpty {
            return false
        }
        print("STICKERS: \(group) \(name)")
        guard let storage = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: GroupIMessageId)?.appendingPathComponent("Library/Caches") else {
            return false
        }
        let groupUrl = storage.appendingPathComponent("\(group)")
        var isDir : ObjCBool = true
        if !FileManager.default.fileExists(atPath: groupUrl.path, isDirectory: &isDir) {
            print("STICKERS: DELETE STICKER ERROR NO GROUP NAME \(group) \(name)")
            return false
        }
        isDir = false
        let fileUrl = storage.appendingPathComponent("\(group)/\(name).png")
        if !FileManager.default.fileExists(atPath: fileUrl.path, isDirectory: &isDir) {
            print("STICKERS: DELETE STICKER ERROR NO GROUP NAME \(group) \(name)")
            return false
        }
        let jsonUrl = storage.appendingPathComponent("stickers.json")
        if let jsonContent = try? Data(contentsOf: jsonUrl) {
            do {
                print("STICKERS: DELETE STICKER TO EXITS GROUP \(group) \(name)")
                try FileManager.default.removeItem(at: fileUrl)
                var stickers = try JSONDecoder().decode([StickerModel].self, from: jsonContent)
                if let sticker = stickers.first(where: { model in
                    return model.group == group
                }) {
                    sticker.icons.removeAll { item in
                        item == name
                    }
                    if sticker.icons.isEmpty {
                        stickers.removeAll { model in
                            model.group == group
                        }
                        var isDir : ObjCBool = true
                        if FileManager.default.fileExists(atPath: groupUrl.path, isDirectory: &isDir) {
                            print("STICKERS: DELETE STICKER ERROR NO GROUP NAME \(group)")
                            try FileManager.default.removeItem(at: groupUrl)
                        }
                    }
                }
                if FileManager.default.fileExists(atPath: jsonUrl.path) {
                    try FileManager.default.removeItem(at: jsonUrl)
                }
                let stickersData = try JSONEncoder().encode(stickers)
                try stickersData.write(to: jsonUrl)
                return true
            } catch {
                return false
            }
        } else {
            do {
                try FileManager.default.removeItem(at: fileUrl)
                return true
            } catch {
                return false
            }
        }
    }
    
    // MARK: Save many stickers to app
    @discardableResult
    @objc public func saveStickers(to group: String, datas: [String: NSData]) -> Bool {
        if group.isEmpty || datas.isEmpty {
            return false
        }
        guard let _ = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: GroupIMessageId)?.appendingPathComponent("Library/Caches") else {
            return false
        }
        datas.forEach { key, value in
            saveSticker(to: group, name: key, data: value)
        }
        return true
    }
    
    // MARK: Delete many stickers from app
    @discardableResult
    @objc public func deleteStickers(to group: String, stickers: [String]) -> Bool {
        if group.isEmpty || stickers.isEmpty {
            return false
        }
        print("STICKERS: \(group)")
        guard let _ = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: GroupIMessageId)?.appendingPathComponent("Library/Caches") else {
            return false
        }
        stickers.forEach { item in
            deleteSticker(from: group, name: item)
        }
        return true
    }
    
    // MARK: Delete geoup of stickers from app
    @discardableResult
    @objc public func deleteStickerGroup(with group: String) -> Bool {
        if group.isEmpty {
            return false
        }
        print("STICKERS: \(group)")
        guard let storage = FileManager.default.containerURL(forSecurityApplicationGroupIdentifier: GroupIMessageId)?.appendingPathComponent("Library/Caches") else {
            return false
        }
        let groupUrl = storage.appendingPathComponent("\(group)")
        var isDir : ObjCBool = true
        if FileManager.default.fileExists(atPath: groupUrl.path, isDirectory: &isDir) {
            print("STICKERS: DELETE STICKER ERROR NO GROUP NAME \(group)")
            do {
                try FileManager.default.removeItem(at: groupUrl)
            } catch {
                return false
            }
        }
        let jsonUrl = storage.appendingPathComponent("stickers.json")
        if let jsonContent = try? Data(contentsOf: jsonUrl) {
            do {
                var stickers = try JSONDecoder().decode([StickerModel].self, from: jsonContent)
                stickers.removeAll() { item in
                    return item.group == group
                }
                
                if FileManager.default.fileExists(atPath: jsonUrl.path) {
                    try FileManager.default.removeItem(at: jsonUrl)
                }
                let stickersData = try JSONEncoder().encode(stickers)
                try stickersData.write(to: jsonUrl)
                return true
            } catch {
                return false
            }
        }
        return false
    }
    
    // MARK: TEST COMMAND
    @objc public func saveSticker(data: NSData) {
        let group = "New"
        let name = UUID().uuidString
        print(saveSticker(to: group, name: name, data: data) ? "STICKERS: SAVE SUCESS" : "STICKERS: SAVE ERROR")
    }
    
}

