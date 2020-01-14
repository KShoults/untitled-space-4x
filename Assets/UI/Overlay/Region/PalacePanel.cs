using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PalacePanel : MonoBehaviour
{
    public Palace palace;
    public Slider civilianDemandSlider, militaryDemandSlider, shipDemandSlider;
    public TextMeshProUGUI civilianTurnReceived, civilianTotalReceived, militaryTurnReceived, militaryTotalReceived, shipTurnReceived, shipTotalReceived,
        popTurnSent, popTotalSent;

    void OnEnable()
    {
        if (palace == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // Update texts
        civilianTurnReceived.text = "Last Turn: " + palace.civilianTurnReceived;
        civilianTotalReceived.text = "Total: " + palace.civilianTotalReceived;
        militaryTurnReceived.text = "Last Turn: " + palace.militaryTurnReceived;
        militaryTotalReceived.text = "Total: " + palace.militaryTotalReceived;
        shipTurnReceived.text = "Last Turn: " + palace.shipTurnReceived;
        shipTotalReceived.text = "Total: " + palace.shipTotalReceived;
        popTurnSent.text = "Pop Sent Last Turn: " + palace.popTurnSent;
        popTotalSent.text = "Pop Sent Total: " + palace.popTotalSent;

        // Set the sliders to their current values
        civilianDemandSlider.value = palace.civilianDemand;
        militaryDemandSlider.value = palace.militaryDemand;
        shipDemandSlider.value = palace.shipDemand;
    }

    public void SetCivilianDemand()
    {
        if (palace != null)
        {
            palace.civilianDemand = (int)civilianDemandSlider.value;
        }
    }

    public void SetMilitaryDemand()
    {
        if (palace != null)
        {
            palace.militaryDemand = (int)militaryDemandSlider.value;
        }
    }

    public void SetShipDemand()
    {
        if (palace != null)
        {
            palace.shipDemand = (int)shipDemandSlider.value;
        }
    }
}
