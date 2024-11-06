using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Tests.Fixtures
{
    public class KernelCommonFixture
    {
        public Guid DefaultCustomerOid { get; protected set; }
        public IOwnerApplicationSettings OwnerApplicationSettings { get; protected set; }
        public IDocumentHeader SampleDocumentHeaderWith3Lines { get; protected set; }

        public KernelCommonFixture()
        {
            Mock<IOwnerApplicationSettings> mockOwnerApplicationSettings = new Mock<IOwnerApplicationSettings>(MockBehavior.Strict);


            OwnerApplicationSettings = mockOwnerApplicationSettings.Object;
            DefaultCustomerOid = Guid.Parse("EF8779DD-A6D3-4ED6-AB67-EF3D02C731C6");

        }
    }
}
