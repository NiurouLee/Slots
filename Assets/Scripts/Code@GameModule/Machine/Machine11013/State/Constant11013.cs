using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class Constant11013
    {
        public static readonly uint PinkScatterElement = 11;

        public static readonly uint GoldenScatterElement = 12;

        public static readonly List<uint> ListAllScatterElements = new List<uint>()
        {
            11, 12
        };

        public static readonly uint StarElement = 13;

        public static readonly uint WildBaseElement = 14;

        public static readonly uint WildFreeElement = 15;


        public static async Task ClearStar(MachineContext context)
        {
            var wheel = context.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var listElement = wheel.GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == StarElement)
                {
                    return true;
                }

                return false;
            });

            List<Task> listTask = new List<Task>();
            foreach (var elementContainer in listElement)
            {
                listTask.Add(elementContainer.PlayElementAnimationAsync("Fly"));
            }

            if (listTask.Count > 0)
            {
                await Task.WhenAll(listTask);
            }
        }
    }
}