// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Security.Cryptography;

namespace Lists {

    // Generates (once) and persists a P-256 ECDSA keypair identifying this
    // device to the server. The private key is stored via the same secure
    // vault used for the PIN hash/device secret (ISecretVault); the public
    // key is exported as the raw uncompressed point (0x04 || X || Y, 65
    // bytes, base64-encoded) the server's /register and /deleteUser expect.
    //
    // D, Q.X and Q.Y are stored together (rather than using
    // ECDsa.ExportECPrivateKey()/ImportECPrivateKey(), added later to
    // ECDsa than the rest of this API) so re-import only ever needs the
    // widely-supported ExportParameters/ECDsa.Create(ECParameters) path.
    public static class DeviceKeyPair {
        public const string VaultKey = "device_ecdsa_keypair";
        private const int ComponentLength = 32; // P-256 field element size, bytes

        public static string GetOrCreatePublicKeyBase64()
        {
            ISecretVault vault = SecretVaultFactory.Get();

            if (vault.TryLoad(VaultKey, out byte[] stored) && stored.Length == ComponentLength * 3) {
                byte[] x = new byte[ComponentLength];
                byte[] y = new byte[ComponentLength];
                Array.Copy(stored, ComponentLength, x, 0, ComponentLength);
                Array.Copy(stored, ComponentLength * 2, y, 0, ComponentLength);
                return EncodePublicKey(x, y);
            }

            using (ECDsa ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256)) {
                ECParameters parameters = ecdsa.ExportParameters(true);

                byte[] blob = new byte[ComponentLength * 3];
                Array.Copy(parameters.D, 0, blob, 0, ComponentLength);
                Array.Copy(parameters.Q.X, 0, blob, ComponentLength, ComponentLength);
                Array.Copy(parameters.Q.Y, 0, blob, ComponentLength * 2, ComponentLength);
                vault.Save(VaultKey, blob);

                return EncodePublicKey(parameters.Q.X, parameters.Q.Y);
            }
        }

        private static string EncodePublicKey(byte[] x, byte[] y)
        {
            byte[] rawPoint = new byte[1 + x.Length + y.Length];
            rawPoint[0] = 0x04;
            Array.Copy(x, 0, rawPoint, 1, x.Length);
            Array.Copy(y, 0, rawPoint, 1 + x.Length, y.Length);
            return Convert.ToBase64String(rawPoint);
        }
    }
}
