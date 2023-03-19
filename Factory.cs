using System;
using System.Reflection;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using LiveSplit.SonicFrontiers;

[assembly: ComponentFactory(typeof(SonicFrontiersFactory))]

namespace LiveSplit.SonicFrontiers
{
    public class SonicFrontiersFactory : IComponentFactory
    {
        public string ComponentName => "Sonic Frontiers - Autosplitter";
        public string Description => "Automatic splitting and Game Time calculation";
        public ComponentCategory Category => ComponentCategory.Control;
        public string UpdateName => this.ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/SonicSpeedrunning/LiveSplit.SonicFrontiers/main/";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        public string XMLURL => this.UpdateURL + "Components/update.LiveSplit.SonicFrontiers.xml";
        public IComponent Create(LiveSplitState state) { return new SonicFrontiersComponent(state); }
    }
}