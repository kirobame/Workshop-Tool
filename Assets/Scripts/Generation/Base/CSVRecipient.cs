using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

// Class holding the data of a google sheet which has been published to the web.
public class CSVRecipient : ScriptableObject
{
    public IReadOnlyList<Sheet> Sheets => sheets;
    
    // The address at which the index sheet can be retrieved,
    // with the indices, all other sheet can downloaded by modifying this address. 
    [SerializeField] private string baseUrl;
    
    // Cached data before build. 
    [SerializeField, HideInInspector] private Sheet[] sheets = Array.Empty<Sheet>();

    public void SetSheets(Sheet[] sheets) => this.sheets = sheets;

    //------------------------------------------------------------------------------------------------------------------
    
    // Methods allowing to process manually the download of the sheets.
    public UnityWebRequest GetIdRequest()
    {
        var baseRequest = UnityWebRequest.Get(baseUrl);
        baseRequest.SendWebRequest();

        return baseRequest;
    }
    public UnityWebRequest[] GetRequests(string[] ids)
    {
        var requests = new UnityWebRequest[ids.Length];
        for (var i = 0; i < ids.Length; i++)
        {
            var url = baseUrl;
            var insertionIndex = url.IndexOf("pub?", StringComparison.Ordinal);

            url = url.Insert(insertionIndex + 4, $"gid={ids[i]}&single=true&");
            requests[i] = UnityWebRequest.Get(url);
            requests[i].SendWebRequest();
        }

        return requests;
    }

    //------------------------------------------------------------------------------------------------------------------
    
    // Retrieves all indices (Sheet Ids) from the index sheet. 
    public bool EvaluateIdSheet(string data, out string[] ids)
    {
        if (data.Substring(0, 7) != "Indexes") Debug.LogError("Id sheet was not parameterized to be considered like one.");
        else
        {
            var split = data.Split(',');
                    
            ids = new string[split.Length - 1];
            for (var i = 1; i < split.Length; i++) ids[i - 1] = split[i];
            
            return true;
        }

        ids = null;
        return false;
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    // Runtime method to download all sheets. 
    public IEnumerator Download(Action<Sheet[]> onCompleted)
    {
        var ids = Array.Empty<string>();
        var success = false;
        
        using (var baseRequest = UnityWebRequest.Get(baseUrl))
        {
            yield return baseRequest.SendWebRequest();

            if (baseRequest.isNetworkError) Debug.LogError( $"Download Error : {baseRequest.error}. Could not retrieve Id sheet");
            else
            {
                Debug.Log($"Download success for Id sheet : {baseRequest.downloadHandler.text}");
                success = EvaluateIdSheet(baseRequest.downloadHandler.text, out ids);
            }
        }

        if (!success)
        {
            onCompleted(sheets);
            yield break;
        }

        var newSheets = new List<Sheet>();
        for (var i = 0; i < ids.Length; i++)
        {
            var url = baseUrl;
            var insertionIndex = url.IndexOf("pub?", StringComparison.Ordinal);

            url = url.Insert(insertionIndex + 4, $"gid={ids[i]}&single=true&");
            
            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
            
                if (request.isNetworkError) 
                {
                    Debug.LogError( "Download Error : " + request.error );

                    if (i < sheets.Length)
                    {
                        Debug.LogWarning( $"Using stale data version : {sheets[i].Version} for sheet {sheets[i].Name}");
                        newSheets.Add(sheets[i]);
                    }
                }
                else
                {
                    Debug.Log($"Download success : {request.downloadHandler.text}");

                    var versionSection = request.downloadHandler.text.Split(',').First();
                    UnityEngine.Assertions.Assert.IsTrue( versionSection.Contains('='), "Could not find a '=' at the start of the CSV" );

                    var versionText = versionSection.Split('=')[1];
                    Debug.Log( $"Downloaded data version : {versionText}");

                    var sheet = new Sheet();
                    sheet.Process(request.downloadHandler.text);
                    
                    newSheets.Add(sheet);
                }
            }
        }
        
        sheets = newSheets.ToArray();
        onCompleted(sheets);
    }
}