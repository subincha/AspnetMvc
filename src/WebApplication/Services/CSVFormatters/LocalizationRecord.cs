using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Services.CSVFormatters
{
    public class LocalizationRecord
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
        public string LocalizationCulture { get; set; }
        public string ResourceKey { get; set; }
    }
}
