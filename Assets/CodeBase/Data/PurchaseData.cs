using System;
using System.Collections.Generic;

namespace CodeBase.Data
{
  [Serializable]
  public class PurchaseData
  {
    public List<BoughtIAP> BoughtIAPs = new List<BoughtIAP>();
    public Action Changed;

    public void AddPurchase(string id)
    {
      BoughtIAP boughtIAP = Product(id);
      if (boughtIAP != null)
        boughtIAP.Count++;
      else
        BoughtIAPs.Add(new BoughtIAP { IAPid = id, Count = 1 });

      Changed?.Invoke();
    }

    private BoughtIAP Product(string id) =>
      BoughtIAPs.Find(x => x.IAPid == id);
  }
}