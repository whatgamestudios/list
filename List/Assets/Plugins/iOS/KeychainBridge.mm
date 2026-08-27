
#import <Foundation/Foundation.h>
#include "UnityFramework/UnityFramework-Swift.h"

extern "C" {

#pragma mark - Functions

int _keychainSetSecret(const char* account, const unsigned char* bytes, int length) {
    NSString *acct = [[NSString alloc] initWithUTF8String:account];
    NSData *data = [NSData dataWithBytes:bytes length:length];
    BOOL ok = [[SecretKeychain shared] SetSecretWithAccount:acct bytes:data];
    return ok ? 1 : 0;
}

int _keychainGetSecret(const char* account, unsigned char* outBuffer, int bufferLength) {
    NSString *acct = [[NSString alloc] initWithUTF8String:account];
    NSData *data = [[SecretKeychain shared] GetSecretWithAccount:acct];
    if (data == nil) {
        return -1;
    }
    int len = (int)data.length;
    if (len > bufferLength) {
        return -1;
    }
    memcpy(outBuffer, data.bytes, len);
    return len;
}

int _keychainDeleteSecret(const char* account) {
    NSString *acct = [[NSString alloc] initWithUTF8String:account];
    BOOL ok = [[SecretKeychain shared] DeleteSecretWithAccount:acct];
    return ok ? 1 : 0;
}

}
