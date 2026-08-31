// Copyright (c) Whatgame Studios 2024 - 2026
//
// Requires the "Portable.BouncyCastle" (or "BouncyCastle") NuGet package -
// install it via NuGetForUnity (Window > NuGet > Manage NuGet Packages).
// Unity's built-in System.Security.Cryptography.ECDsa is a stub that throws
// NotImplementedException on every platform (Editor included) - BouncyCastle
// is the standard, pure-C# workaround for this well-known Unity gap.
using System;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace Lists {

    // Generates (once) and persists a P-256 (secp256r1) keypair identifying
    // this device to the server. The private scalar is stored via the same
    // secure vault used for the PIN hash/device secret (ISecretVault); the
    // public key is exported as the raw uncompressed point (0x04 || X || Y,
    // 65 bytes, base64-encoded) the server's /register and /deleteUser expect.
    public static class DeviceKeyPair {
        public const string VaultKey = "device_ecdsa_keypair";
        private const int ComponentLength = 32; // P-256 field element size, bytes

        private static readonly X9ECParameters Curve = SecNamedCurves.GetByName("secp256r1");
        private static readonly ECDomainParameters DomainParameters =
            new ECDomainParameters(Curve.Curve, Curve.G, Curve.N, Curve.H);

        public static string GetOrCreatePublicKeyBase64()
        {
            ISecretVault vault = SecretVaultFactory.Get();

            if (vault.TryLoad(VaultKey, out byte[] storedD) && storedD.Length == ComponentLength) {
                Org.BouncyCastle.Math.BigInteger d = new Org.BouncyCastle.Math.BigInteger(1, storedD);
                ECPoint q = DomainParameters.G.Multiply(d).Normalize();
                return EncodePublicKey(q);
            }

            ECKeyPairGenerator generator = new ECKeyPairGenerator();
            generator.Init(new ECKeyGenerationParameters(DomainParameters, new SecureRandom()));
            AsymmetricCipherKeyPair pair = generator.GenerateKeyPair();

            ECPrivateKeyParameters privateKey = (ECPrivateKeyParameters) pair.Private;
            ECPublicKeyParameters publicKey = (ECPublicKeyParameters) pair.Public;

            byte[] dBytes = FixedLength(privateKey.D.ToByteArrayUnsigned(), ComponentLength);
            vault.Save(VaultKey, dBytes);

            return EncodePublicKey(publicKey.Q.Normalize());
        }

        private static string EncodePublicKey(ECPoint q)
        {
            byte[] x = FixedLength(q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned(), ComponentLength);
            byte[] y = FixedLength(q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned(), ComponentLength);

            byte[] rawPoint = new byte[1 + x.Length + y.Length];
            rawPoint[0] = 0x04;
            Array.Copy(x, 0, rawPoint, 1, x.Length);
            Array.Copy(y, 0, rawPoint, 1 + x.Length, y.Length);
            return Convert.ToBase64String(rawPoint);
        }

        // BigInteger.ToByteArrayUnsigned() omits leading zero bytes, so a
        // scalar/coordinate that happens to start with zero bytes comes back
        // shorter than ComponentLength - left-pad it back out, since the raw
        // point format needs fixed-width X/Y (and the vault needs a fixed-width D).
        private static byte[] FixedLength(byte[] bytes, int length)
        {
            if (bytes.Length == length) {
                return bytes;
            }
            byte[] result = new byte[length];
            int copyLength = Math.Min(bytes.Length, length);
            Array.Copy(bytes, bytes.Length - copyLength, result, length - copyLength, copyLength);
            return result;
        }
    }
}
