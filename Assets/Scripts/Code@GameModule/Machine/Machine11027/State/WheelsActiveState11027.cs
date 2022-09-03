namespace GameModule
{
    public class WheelsActiveState11027 : WheelsActiveState
    {
        public WheelsActiveState11027(MachineState machineState)
            : base(machineState)
        {
        }

        public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            var rollCount = wheel.rollCount;
            for (var i = 0; i < rollCount; ++i)
            {
                roll.Find("spiningMask").gameObject.SetActive(true);
                var rollColorMask = roll.Find("spiningMask").Find("BlackMask" + i).gameObject;
                rollColorMask.SetActive(true);
            }
        }

        public void FadeOutRollMask(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }
            var rollCount = wheel.rollCount;
            for (var i = 0; i < rollCount; ++i)
            {
                roll.Find("spiningMask").gameObject.SetActive(true);
                var rollColorMask = roll.Find("spiningMask").Find("BlackMask" + i).gameObject;
                rollColorMask.SetActive(false);
            }
        }
    }
}