using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class CSVUploader : MonoBehaviour
{
    [SerializeField] public SOGameConfiguration gameConfigSO;
    private string baseURL = "https://gkstatsweb.duckdns.org:12345/upload_csv?userId=";
    private static readonly string _filename = "Historical.csv";

    [SerializeField] private GameObject sendingNote;
    [SerializeField] private GameObject okNote;
    [SerializeField] private GameObject wrongNote;


    enum Directories
    {
        CompareFiability,
        ReactionSpeed,
        Tracking
    }

    private void searchForFile(string directoryPath)
    {
        Debug.Log("Searching for file in: " + directoryPath);
        string[] filesInSubdirectory = Directory.GetFiles(directoryPath);
        foreach (string file in filesInSubdirectory)
        {
            if (Path.GetFileName(file).Contains(Constants_Handball.seriesDate.ToString(RehabConstants.DateFormat)))
            {
                Debug.Log("Found file: " + file);
                StartCoroutine(UploadCSV(file));
                break;
            }
        }
    }

    public void OnClick()
    {
        // string directoryPath = Application.persistentDataPath + "/" + Constants_Handball.identificador.ToString(); //gameConfig.gameConfiguration.userId
        string directoryPath = gameConfigSO.userPath;
        string filePath = Path.Combine(directoryPath, _filename);

        sendingNote.SetActive(true);
        okNote.SetActive(false);
        wrongNote.SetActive(false);

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError("--- Directory not found: " + directoryPath);
            return;
        }

        if (File.Exists(filePath))
        {
            StartCoroutine(UploadCSV(filePath));
        }
        else
        {
            Debug.LogError("CSV file not found at: " + filePath);
        }

        string[] subdirectoryEntries = Directory.GetDirectories(directoryPath);

        foreach (string subdirectory in subdirectoryEntries)
        {

            if (subdirectory.Contains(nameof(Directories.Tracking)))
            {
                searchForFile(subdirectory);
            }
            else if (subdirectory.Contains(nameof(Directories.ReactionSpeed)))
            {
                searchForFile(subdirectory);
            }
           /* else if (subdirectory.Contains(nameof(Directories.CompareFiability)))
            {
                searchForFile(subdirectory);
            }
            */
        }
    }

    IEnumerator UploadCSV(string filePath)
    {
        string url = baseURL + UnityWebRequest.EscapeURL(Constants_Handball.identificador.ToString());

        Debug.Log("Starting CSV upload from: " + filePath);

        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);

        // multipart upload
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, fileName, "text/csv");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
      //  request.CertificateHandler = new LetsEncryptCertificateHandler(letsEncryptRootCert);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Upload failed: " + request.error);
            Debug.LogError("response: " + request.downloadHandler.text);
            sendingNote.SetActive(false);
            wrongNote.SetActive(true);
            okNote.SetActive(false);
            yield return new WaitForSeconds(2);
            wrongNote.SetActive(false);
        }
        else if (request.responseCode == 200)
        {
            Debug.Log("Upload successful: " + request.downloadHandler.text);
            sendingNote.SetActive(false);
            okNote.SetActive(true);
            wrongNote.SetActive(false);
            yield return new WaitForSeconds(2);
            okNote.SetActive(false);
        }
        else
        {
            Debug.LogError("Upload failed with response code: " + request.responseCode);
            sendingNote.SetActive(false);
            wrongNote.SetActive(true);
            okNote.SetActive(false);
            yield return new WaitForSeconds(2);
            wrongNote.SetActive(false);
        }
    }
}
