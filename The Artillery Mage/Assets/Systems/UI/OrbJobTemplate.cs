using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OrbJobTemplate : MonoBehaviour
{
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI jobTxt;

    [Space]

    [SerializeField] TextMeshProUGUI timeLeftTxt;

    RequestData _request;

    public void Setup(RequestData request)
	{
        _request = request;

        nameTxt.SetText(request.personRequesting);
        jobTxt.SetText(request.requestDetails);
	}

	private void Update()
	{
        var r = RequestManager.Instance.currentlyActiveRequests.FirstOrDefault(x => x.request == _request);
        timeLeftTxt.SetText(r.timeLeft.ToString("f0") + "s");
    }
}
