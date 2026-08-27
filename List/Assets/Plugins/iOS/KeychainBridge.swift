
import Foundation
import Security

@objc public class SecretKeychain: NSObject {
    @objc public static let shared = SecretKeychain()

    private let service = "com.whatgamestudios.list.devicesecret"

    private func query(account: String) -> [String: Any] {
        return [
            kSecClass as String: kSecClassGenericPassword,
            kSecAttrService as String: service,
            kSecAttrAccount as String: account
        ]
    }

    @objc public func SetSecret(account: String, bytes: NSData) -> Bool {
        SecItemDelete(query(account: account) as CFDictionary)

        var addQuery = query(account: account)
        addQuery[kSecValueData as String] = bytes as Data
        addQuery[kSecAttrAccessible as String] = kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly

        let status = SecItemAdd(addQuery as CFDictionary, nil)
        return status == errSecSuccess
    }

    @objc public func GetSecret(account: String) -> NSData? {
        var getQuery = query(account: account)
        getQuery[kSecReturnData as String] = true
        getQuery[kSecMatchLimit as String] = kSecMatchLimitOne

        var result: AnyObject?
        let status = SecItemCopyMatching(getQuery as CFDictionary, &result)
        guard status == errSecSuccess, let data = result as? Data else {
            return nil
        }
        return data as NSData
    }

    @objc public func DeleteSecret(account: String) -> Bool {
        let status = SecItemDelete(query(account: account) as CFDictionary)
        return status == errSecSuccess || status == errSecItemNotFound
    }
}
