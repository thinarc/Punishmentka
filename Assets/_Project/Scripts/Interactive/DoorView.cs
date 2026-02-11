using _Project.Scripts.EntryPoints;

namespace _Project.Scripts.Interactive
{
    public class DoorView : ItemView
    {
        public override void SeeView()
        {
            FindAnyObjectByType<GameEntryPoint>().DoChangeScene(1);
        }

        public override void UnseeView() { }
    }
}