
#import <Foundation/Foundation.h>
#include "UnityFramework/UnityFramework-Swift.h"

extern "C" {

void UnitySendMessage(const char* obj, const char* method, const char* msg);

#pragma mark - Functions

int _biometricIsAvailable() {
    return [[BiometricAuth shared] IsAvailable] ? 1 : 0;
}

void _biometricAuthenticate(const char* reason, const char* callbackGameObject) {
    NSString *nsReason = [[NSString alloc] initWithUTF8String:reason];
    NSString *goName = [[NSString alloc] initWithUTF8String:callbackGameObject];

    [[BiometricAuth shared] AuthenticateWithReason:nsReason completion:^(BOOL success) {
        UnitySendMessage([goName UTF8String], "OnBiometricResult", success ? "1" : "0");
    }];
}

}
