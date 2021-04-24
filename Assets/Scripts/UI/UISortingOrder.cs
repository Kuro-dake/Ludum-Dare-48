using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISortingOrder : MonoBehaviour
{
    static List<UISortingOrder> all_sortings = new List<UISortingOrder>();

    [SerializeField]
    int order;
    void Start()
    {
        all_sortings.Add(this);
        Sort();
    }

    public static void Sort()
    {
        all_sortings.RemoveAll(s => s == null);
        all_sortings.Sort((a, b) => a.order.CompareTo(b.order));
        all_sortings.ForEach(s => s.GetComponent<RectTransform>().SetAsLastSibling());
        /*string debug = "sorting ui:";
        all_sortings.ForEach(s => debug += s.name + "|");
        Debug.Log(debug);*/

    }

}
