using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class CSVRecipientEditor : Editor
{
    #region Encapsuled Types

    private enum FetchPhase
    {
        None,
        Ids,
        Sheets,
    }
    #endregion
    
    private CSVRecipient Recipient => (CSVRecipient)serializedObject.targetObject;
    
    private UnityWebRequest[] requests;
    private FetchPhase fetchPhase = FetchPhase.None;
    
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        var iterator = serializedObject.GetIterator();
        iterator.NextVisible(true);

        GUI.enabled = false;
        EditorGUILayout.PropertyField(iterator);
        GUI.enabled = true;

        iterator.NextVisible(false);
        EditorGUILayout.PropertyField(iterator);

        if (fetchPhase != FetchPhase.None) GUI.enabled = false;

        if (GUILayout.Button("Fetch"))
        {
            requests = new UnityWebRequest[] {Recipient.GetIdRequest()};
            fetchPhase = FetchPhase.Ids;
            
            ToggleInspectorLock();
            EditorApplication.update += UpdateRequests;
        }
        GUI.enabled = true;
        
        if (!Recipient.Sheets.Any() || Recipient.Sheets.Any(sheet => !sheet.IsInitialized)) EditorGUILayout.HelpBox("Recipient has no full backup value for the moment! Please fetch.", MessageType.Warning);
        else if (GUILayout.Button("Display backup")) { foreach (var sheet in Recipient.Sheets) Debug.Log(sheet); }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateRequests()
    {
        Debug.Log("Updating requests");
        
        if (fetchPhase == FetchPhase.Ids)
        {
            if (!requests.First().isDone) return;
            
            var baseRequest = requests.First();
            if (baseRequest.isNetworkError) Debug.LogError($"Network error : {baseRequest.error}");
            else
            {
                Debug.Log($"Id sheet download was a success : {baseRequest.downloadHandler.text}");
                if (Recipient.EvaluateIdSheet(baseRequest.downloadHandler.text, out var ids))
                {
                    requests = Recipient.GetRequests(ids);
                    fetchPhase = FetchPhase.Sheets;
                }
                else fetchPhase = FetchPhase.None;
            }
        }
        
        if (fetchPhase == FetchPhase.Sheets)
        {
            if (requests.Any(request => !request.isDone)) return;
            
            if (requests.Any(request => request.isNetworkError)) Debug.LogError("Network error !");
            else
            {
                var sheets = new Sheet[requests.Length];
                for (var i = 0; i < requests.Length; i++)
                {
                    var request = requests[i];
                    
                    Debug.Log($"Download successful : {request.downloadHandler.text}");

                    sheets[i] = new Sheet();
                    sheets[i].Process(request.downloadHandler.text);
                }
                
                Recipient.SetSheets(sheets);
            }
            
            fetchPhase = FetchPhase.None;
        }

        ToggleInspectorLock();
        EditorApplication.update -= UpdateRequests;
    }
    
    private void ToggleInspectorLock()
    {
        var inspectorToBeLocked = EditorWindow.mouseOverWindow; 
 
        var projectBrowserType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.ProjectBrowser");
        var inspectorWindowType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
 
        PropertyInfo propertyInfo;
        if (inspectorToBeLocked.GetType() == projectBrowserType)
        {
            propertyInfo = projectBrowserType.GetProperty("isLocked", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        else if (inspectorToBeLocked.GetType() == inspectorWindowType)
        {
            propertyInfo = inspectorWindowType.GetProperty("isLocked");
        }
        else
        {
            return;
        }
 
        var value = (bool)propertyInfo.GetValue(inspectorToBeLocked, null);
        propertyInfo.SetValue(inspectorToBeLocked, !value, null);
        inspectorToBeLocked.Repaint();
    }
}