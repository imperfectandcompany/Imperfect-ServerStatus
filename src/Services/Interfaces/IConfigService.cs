using IGDiscord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGDiscord.Services.Interfaces
{
    public interface IConfigService
    {
        Config? LoadConfig(string moduleDirectory);

        void UpdateConfig(Config configData);
    }
}
