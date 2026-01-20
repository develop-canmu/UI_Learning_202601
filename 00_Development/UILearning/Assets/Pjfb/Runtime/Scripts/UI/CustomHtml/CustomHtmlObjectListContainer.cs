using System.Linq;
using Pjfb.UI;
using Pjfb.Utility;

namespace Pjfb.CustomHtml
{
    public class CustomHtmlObjectListContainer : ListContainer
    {
        #region ContainerParams
        public class ContainerParams
        {
            public string htmlBody;
        }
        #endregion
        
        public void SetDisplay(ContainerParams parameters)
        {
            var customHtmlConvertResult = CustomHtmlManager.ConvertCustomHtmlString(parameters.htmlBody);
            var htmlObjectParams = customHtmlConvertResult.objectParamList;
            SetDataList(htmlObjectParams.Select(aData => new CustomHtmlObjectListItem.ItemParams {objectParam = aData, onClickButton = OnClickButton}).ToList());
        }

        private void OnClickButton(string param)
        {
            ServerActionCommandUtility.ProceedAction(param);
        }
    }
}
