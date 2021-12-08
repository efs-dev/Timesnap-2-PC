using Zenject;

namespace EFS.Timesnap.VR
{
    public class TimesnapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DeviceDetector>().FromNew().AsSingle();
        }
    }
}