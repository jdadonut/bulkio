using System;
using System.Collections.Generic;
namespace bulkio.Entities
{
    public class CensorshipOptions
    {
        public bool Use;
        public bool HasSafe, HasQuestionable, HasExplicit;
        public DetectionMethod Method;
    }
}
