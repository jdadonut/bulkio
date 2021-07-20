using System;
using System.Collections.Generic;
namespace bulkio.Entities
{
    class CensorshipOptions
    {
        public bool Use;
        public bool HasSafe, HasQuestionable, HasExplicit;
        public DetectionMethod Method;
    }
}
