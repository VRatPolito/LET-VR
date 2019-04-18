using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabDestination : Destination
{
    [SerializeField] private List<GenericItem> _items;
    [SerializeField] private bool _autoDisable = true;
    [SerializeField] private bool _makeItemsUngrabbable = true;
    [SerializeField] private bool _makeKinematic = false;

    private void OnTriggerEnter(Collider c)
    {
		var item = c.GetComponent<GenericItem>();
        if (item != null && _items.Contains(item))
        {
			var p = item.Player;
			if(_makeKinematic)
				item.IsKinematic = true;
            if(p != null)
			    p.DropItem(c.transform, true);
            if(_makeItemsUngrabbable)
				item.CanInteract(false, LocomotionManager.Instance.CurrentPlayerController.GetComponent<VRItemController>());
            if(_autoDisable)
                gameObject.SetActive(false);
        }
    }
}
