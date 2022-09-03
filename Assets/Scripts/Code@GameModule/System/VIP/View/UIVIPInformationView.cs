using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIVIPInformation")]
    public class UIVIPInformationView : Popup
    {
        public UIVIPInformationView(string addrass) : base(addrass)
        {
            
        }
        
        
        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return new Vector3(0.55f, 0.55f, 0.55f);
            }
            else
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    float scale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    return Vector3.one * scale;
                }
            }

            return Vector3.one;
        }
        
        
    }
}