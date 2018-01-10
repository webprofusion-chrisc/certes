﻿using System.Threading.Tasks;
using Certes.Acme;
using Certes.Acme.Resource;
using Certes.Pkcs;
using Moq;
using Xunit;

namespace Certes
{
    public class IOrderContextExtensionsTests
    {
        [Fact]
        public async Task CanGenerateCertificate()
        {
            var pem = "certes-pem";

            var authzCtxMock = new Mock<IAuthorizationContext>();
            authzCtxMock.Setup(m => m.Resource())
                .ReturnsAsync(new Acme.Resource.Authorization
                {
                    Identifier = new Identifier
                    {
                        Type = IdentifierType.Dns,
                        Value = "www.certes.com",
                    }
                });

            var orderCtxMock = new Mock<IOrderContext>();
            orderCtxMock.Setup(m => m.Finalize(It.IsAny<byte[]>()))
                .ReturnsAsync(new Order());
            orderCtxMock.Setup(m => m.Download()).ReturnsAsync(pem);
            orderCtxMock.Setup(m => m.Authorizations()).ReturnsAsync(new[] { authzCtxMock.Object });

            var certInfoWithRandomKey = await orderCtxMock.Object.Generate(new CsrInfo
            {
                CountryName = "C",
                CommonName = "www.certes.com",
            });

            Assert.Equal(pem, certInfoWithRandomKey.ToPem());
            Assert.NotNull(certInfoWithRandomKey.PrivateKey);

            var key = DSA.NewKey(SignatureAlgorithm.RS256);
            var certInfo = await orderCtxMock.Object.Generate(new CsrInfo
            {
                CountryName = "C",
                CommonName = "www.certes.com",
            }, key);

            Assert.Equal(pem, certInfo.ToPem());
            Assert.Equal(key, certInfo.PrivateKey);

            var certInfoNoCn = await orderCtxMock.Object.Generate(new CsrInfo
            {
                CountryName = "C",
            });

            Assert.Equal(pem, certInfoNoCn.ToPem());
            Assert.NotNull(certInfoNoCn.PrivateKey);
        }
    }

}