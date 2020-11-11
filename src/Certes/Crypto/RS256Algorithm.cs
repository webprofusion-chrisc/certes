﻿using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Certes.Crypto
{
    internal sealed class RS256Algorithm : IKeyAlgorithm
    {
        public ISigner CreateSigner(IKey key) => new RS256Signer(key);

        public IKey GenerateKey(int? strength = 2048)
        {
            if (strength == null)
            {
                strength = 2048;
            }

            var generator = GeneratorUtilities.GetKeyPairGenerator("RSA");
            var generatorParams = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(0x10001), new SecureRandom(), (int)strength, 128);
            generator.Init(generatorParams);
            var keyPair = generator.GenerateKeyPair();
            return new AsymmetricCipherKey(KeyAlgorithm.RS256, keyPair);
        }
    }
}
