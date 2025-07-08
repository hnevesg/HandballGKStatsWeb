using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuControllerHands : MonoBehaviour
{
    public ToggleInfo[] shootPositions;
    public TMPro.TMP_Text warningLabel;
    public TMPro.TMP_Text identificadorSeleccionado;
    private CanvasController canvasController;
    private LoadConfiguration loadConfiguration;
    private bool DifficultyChanged = false;
    [SerializeField] private SOGameConfiguration gameConfig;
    [SerializeField] GameObject Porteria;
    [SerializeField] GameObject PorteriaAdaptada;

    //Referencias a componentes afectados por configuracion
    /*********************Toggles manos*****************/
    [SerializeField] private Toggle AmbasManos;
    [SerializeField] private Toggle ManoDerecha;
    [SerializeField] private Toggle ManoIzquierda;
    /**************************************************/
    /*********************Toggles goles objetivo*****************/
    [SerializeField] private Toggle Goles5;
    [SerializeField] private Toggle Goles10;
    [SerializeField] private Toggle Goles15;
    [SerializeField] private Toggle Goles20;
    /**************************************************/
    /*********************Toggles modos*****************/
    [SerializeField] private Toggle LevelPrincipiante;
    [SerializeField] private Toggle LevelIntermedio;
    [SerializeField] private Toggle LevelExperto;
    [SerializeField] private Toggle LevelProgresivo;
    [SerializeField] private Toggle LevelProgresivo2;
    /**************************************************/
    [SerializeField] private Toggle ToggleCentral;
    [SerializeField] private Toggle TogglePivote;
    [SerializeField] private Toggle ToggleLatIzq;
    [SerializeField] private Toggle ToggleLatDer;
    [SerializeField] private Toggle ToggleExtIzq;
    [SerializeField] private Toggle ToggleExtDer;
    [SerializeField] private Toggle TogggleAdaptedHandball;
    [SerializeField] private Toggle ToggleDesplazamientoCanon;
    [SerializeField] private Toggle ToggleCurvaLanzamiento;
    [SerializeField] private Toggle ToggleRotacionPelota;
    [SerializeField] private Toggle ToggleDistintoTamanoPelota;

    void Start()
    {
        canvasController = FindObjectOfType<CanvasController>();
        loadConfiguration = FindObjectOfType<LoadConfiguration>();
        warningLabel.gameObject.SetActive(false);
        foreach (var positionInfo in shootPositions)
        {
            positionInfo.toggle.onValueChanged.AddListener((value) => OnToggleValueChanged(positionInfo.type, value));
        }
        CheckIfUserConfigExists();
        //BORRAR
        //Constants_Handball.Difficulty = Difficulty.Progressive2;
        //canvasController.ComenzarCuentaAtras();
        //StartBtn();
    }
    public void StartBtn()
    {
        configureHandBallType();

        if (!DifficultyChanged)
        {
            Constants_Handball.Difficulty = Difficulty.Beginner;
            if (isAtLeastOnePositionSelected())
            {
                canvasController.ComenzarCuentaAtras(false);
            }
        }
        else
        {
            if (isAtLeastOnePositionSelected())
            {
                canvasController.ComenzarCuentaAtras(false);
            }
        }
        CreateDirectoryIfNecessary();
    }
    public void OnToggleDificultadValueChanged(int index)
    {
        if (!DifficultyChanged)
        {
            DifficultyChanged = true;
        }

        switch (index)
        {
            case 0:
                Constants_Handball.Difficulty = Difficulty.Beginner;
                gameConfig.gameConfiguration.level = 1;
                break;
            case 1:
                Constants_Handball.Difficulty = Difficulty.Intermediate;
                gameConfig.gameConfiguration.level = 2;
                break;
            case 2:
                Constants_Handball.Difficulty = Difficulty.Expert;
                gameConfig.gameConfiguration.level = 3;
                break;
            case 3:
                Constants_Handball.Difficulty = Difficulty.Progressive1;
                gameConfig.gameConfiguration.level = 4;
                break;
            case 4:
                Constants_Handball.Difficulty = Difficulty.Progressive2;
                gameConfig.gameConfiguration.level = 5;
                break;
        }
    }
    public void ToggleManosUtilizadasValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                Constants_Handball.handsType = HandsType.Both;
                gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_manosUtilizadas, "both");
                break;
            case 1:
                Constants_Handball.handsType = HandsType.Right;
                gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_manosUtilizadas, "right");
                break;
            case 2:
                Constants_Handball.handsType = HandsType.Left;
                gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_manosUtilizadas, "left");
                break;
        }
    }
    public void OnTogleGolesObjetivoValueChanged(int index)
    {
        int value = index * 5 + 5;
        Constants_Handball.n_limit_goles = value;
        Constants_Handball.n_limit_goles_index = index;
    }

    public void onToggleChange(bool tog)
    {
        if (tog)
        {
            Constants_Handball.HandballType = HandballType.AdaptedHandball;
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_handballadapted, "true");
        }
        else
        {
            Constants_Handball.HandballType = HandballType.Handball; 
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_handballadapted, "false");
        }
    }
    public void onToggleDesplazamientoCanonChange(bool tog)
    {
        Constants_Handball.isActiveDesplazamientoCanon = tog;
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_desplazamientoCanon, tog.ToString());
    }
    public void onToggleRotacionPelota(bool tog)
    {
        Constants_Handball.isActiveRotacionPelota = tog;
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_rotacionPelota, tog.ToString());
    }
    public void onToggleCurvaLanzamiento(bool tog)
    {
        Constants_Handball.isActiveCurvaLanzamiento = tog;
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_curvaLanzamiento, tog.ToString());
    }
    public void onToggleDistintosTiposPelotas(bool tog)
    {
        Constants_Handball.isActiveDistintosTiposPelotas = tog;
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_distintosTiposPelotas, tog.ToString());
    }

    private void configureHandBallType()
    {
        switch (Constants_Handball.HandballType)
        {
            case HandballType.Handball:
                Porteria.SetActive(true);
                PorteriaAdaptada.SetActive(false);
                break;
            case HandballType.AdaptedHandball:
                Porteria.SetActive(false);
                PorteriaAdaptada.SetActive(true);
                break;
            default:
                Porteria.SetActive(true);
                PorteriaAdaptada.SetActive(false);
                break;
        }
    }

    public void OnDropdownIdentificadorValueChanged(int value)
    {
        Constants_Handball.identificador = value;
        gameConfig.gameConfiguration.userId = Constants_Handball.identificador.ToString();
        identificadorSeleccionado.text = Constants_Handball.identificador.ToString();
        CheckIfUserConfigExists();
    }
    private void OnToggleValueChanged(ShootPositions type, bool value)
    {
        switch (type)
        {
            case ShootPositions.Central:
                Constants_Handball.centralCanonActivated = value;
                break;
            case ShootPositions.Pivote:
                Constants_Handball.pivotCanonActivated = value;
                break;
            case ShootPositions.LateralIzquierdo:
                Constants_Handball.latIzqCanonActivated = value;
                break;
            case ShootPositions.LateralDerecho:
                Constants_Handball.latDerCanonActivated = value;
                break;
            case ShootPositions.ExtremoIzquierdo:
                Constants_Handball.extIzqCanonActivated = value;
                break;
            case ShootPositions.ExtremoDerecho:
                Constants_Handball.extDerCanonActivated = value;
                break;
        }
    }

    /*Funcion para comprobar que al menos una opcion esta seleccionada*/
    public bool isAtLeastOnePositionSelected()
    {
        if (Constants_Handball.centralCanonActivated || Constants_Handball.pivotCanonActivated || Constants_Handball.latIzqCanonActivated || Constants_Handball.latDerCanonActivated ||
            Constants_Handball.extIzqCanonActivated || Constants_Handball.extDerCanonActivated)
        {
            return true;
        }
        else
        {
            StartCoroutine(showWarning());
            return false;
        }
    }
    IEnumerator showWarning()
    {
        warningLabel.gameObject.SetActive(true);    // Activamos el texto
        yield return new WaitForSeconds(3.0f);      // Esperamos la duraci�n especificada
        warningLabel.gameObject.SetActive(false);   // Desactivamos el texto despu�s de la duraci�n
    }

    private void updateAllConfigurationInfoBeforeSave(){
        // Dificultad
        switch (Constants_Handball.Difficulty)
        {
            case Difficulty.Beginner:
                gameConfig.gameConfiguration.level = 1;
                break;
            case Difficulty.Intermediate:
                gameConfig.gameConfiguration.level = 2;
                break;
            case Difficulty.Expert:
                gameConfig.gameConfiguration.level = 3;
                break;
            case Difficulty.Progressive1:
                gameConfig.gameConfiguration.level = 4;
                break;
            case Difficulty.Progressive2:
                gameConfig.gameConfiguration.level = 5;
                break;
        }

        //Balonmano adaptado
        if (Constants_Handball.HandballType == HandballType.AdaptedHandball)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_handballadapted, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_handballadapted, "false");
        }

        //Desplazamiento canon
        if (Constants_Handball.isActiveDesplazamientoCanon)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_desplazamientoCanon, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_desplazamientoCanon, "false");
        }

        //Rotacion pelota
        if (Constants_Handball.isActiveRotacionPelota)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_rotacionPelota, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_rotacionPelota, "false");
        }

        //Curva lanzamiento
        if (Constants_Handball.isActiveCurvaLanzamiento)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_curvaLanzamiento, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_curvaLanzamiento, "false");
        }
        
        //Distintos tipo de pelota
        if (Constants_Handball.isActiveDistintosTiposPelotas)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_distintosTiposPelotas, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_distintosTiposPelotas, "false");
        }

        //Manos
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_manosUtilizadas, Constants_Handball.handsType.ToString());

        //Goles objetivo
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_golesObjetivo, Constants_Handball.n_limit_goles.ToString());
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_golesObjetivoIndex, Constants_Handball.n_limit_goles_index.ToString());

        //Identificador
        gameConfig.gameConfiguration.userId = Constants_Handball.identificador.ToString();

        //Identificador ultimo ejercicio

        Constants_Handball.idEjercicio = Constants_Handball.idEjercicio + 1;
        gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_idEjercicio, Constants_Handball.idEjercicio.ToString());

        //Hand Interaction
        gameConfig.gameConfiguration.handInteraction = GameConfiguration.HandInteraction.Both;

        //Posiciones disparo
        if (Constants_Handball.centralCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonCentral, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonCentral, "false");
        }

        if (Constants_Handball.pivotCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonPivote, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonPivote, "false");
        }

        if (Constants_Handball.latIzqCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonLatIzq, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonLatIzq, "false");
        }

        if (Constants_Handball.latDerCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonLatDer, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonLatDer, "false");
        }

        if (Constants_Handball.extIzqCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonExtIzq, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonExtIzq, "false");
        }

        if (Constants_Handball.extDerCanonActivated)
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonExtDer, "true");
        }
        else
        {
            gameConfig.gameConfiguration.AddOrUpdateOption(KeysConfiguration.k_canonExtDer, "false");
        }
    }
    private void CreateDirectoryIfNecessary()
    {
        gameConfig.resetAll();
        string idUser = Constants_Handball.identificador.ToString();
        gameConfig.gameConfiguration.userId = idUser;
        DirectoryInfo dirParent = Directory.GetParent(Application.persistentDataPath);
        if (dirParent != null)
        {
            if (gameConfig.gameName == null)
            {
                Debug.Log("No se ha podido encontrar el directorio del juego. La ruta contendr� el directorio por defecto None");
                gameConfig.gameName = "None";
            }
            string parentPath = dirParent.ToString();
            string userPath = Path.Combine(parentPath, gameConfig.gameName, idUser);
            gameConfig.userPath = userPath;
            String userFileConfiguration = Path.Combine(gameConfig.userPath, RehabConstants.FileConfiguration);

            if (!Directory.Exists(userPath))
            {
                try
                {
                    //create new user path
                    Directory.CreateDirectory(userPath);
                    SaveConfiguration();
                }
                catch (Exception e)
                {
                    Debug.LogError("Could not create user configuration folder: " + e.Message);
                }
            }
            else if (File.Exists(userFileConfiguration))
            {
                SaveConfiguration();
            }
        }

    }
    private void SaveConfiguration()
    {
        updateAllConfigurationInfoBeforeSave();
        try
        {
            DirectoryInfo dirParent = Directory.GetParent(Application.persistentDataPath);
            if (dirParent != null)
            {
                if (gameConfig.gameName == null)
                {
                    Debug.Log("No se ha podido encontrar el directorio del juego. La ruta contendr� el directorio por defecto None");
                    gameConfig.gameName = "None";
                }
                string parentPath = dirParent.ToString();
                string userPath = Path.Combine(parentPath, gameConfig.gameName, gameConfig.gameConfiguration.userId);
                gameConfig.userPath = userPath;
                String userFileConfiguration = Path.Combine(gameConfig.userPath, RehabConstants.FileConfiguration);
                Debug.Log(userFileConfiguration);
                string dataString = JsonUtility.ToJson(gameConfig.gameConfiguration);
                File.WriteAllText(userFileConfiguration, dataString);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error saving user configuration " + e.Message);
        }
    }
    private void CheckIfUserConfigExists()
    {
        gameConfig.resetAll();
        string idUser = Constants_Handball.identificador.ToString();
        gameConfig.gameConfiguration.userId = idUser;
        //search idUser into path to load configuration
        DirectoryInfo dirParent = Directory.GetParent(Application.persistentDataPath);
        if (dirParent != null)
        {
            if (gameConfig.gameName == null)
            {
                Debug.Log("No se ha podido encontrar el directorio del juego. La ruta contendr� el directorio por defecto None");
                gameConfig.gameName = "None";
            }
            string parentPath = dirParent.ToString();
            string userPath = Path.Combine(parentPath, gameConfig.gameName, idUser);
            gameConfig.userPath = userPath;
            String userFileConfiguration = Path.Combine(gameConfig.userPath, RehabConstants.FileConfiguration);
            if (File.Exists(userFileConfiguration))
            {
                loadConfiguration.LoadConfigurationFromFile(userFileConfiguration);
                updateNewConfigurationInComponentes();
            }
        }

    }

    private void updateNewConfigurationInComponentes()
    {
        switch (gameConfig.gameConfiguration.level)
        {
            case 1:
                Constants_Handball.Difficulty = Difficulty.Beginner;
                LevelPrincipiante.isOn = true;
                break;
            case 2:
                Constants_Handball.Difficulty = Difficulty.Intermediate;
                LevelIntermedio.isOn = true;
                break;
            case 3:
                Constants_Handball.Difficulty = Difficulty.Expert;
                LevelExperto.isOn = true;
                break;
            case 4:
                Constants_Handball.Difficulty = Difficulty.Progressive1;
                LevelProgresivo.isOn = true;
                break;
            case 5:
                Constants_Handball.Difficulty = Difficulty.Progressive2;
                LevelProgresivo2.isOn = true;
                break;
            default:
                Constants_Handball.Difficulty = Difficulty.Beginner;
                LevelPrincipiante.isOn = true;
                break;
        }

        switch (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_manosUtilizadas])
        {
            case "both":
                AmbasManos.isOn = true;
                break;
            case "left":
                ManoIzquierda.isOn = true;
                break;
            case "right":
                ManoDerecha.isOn = true;
                break;
        }

        int golesObjetivoValue = int.Parse(gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_golesObjetivo]);
        int golesObjetivoIndex = int.Parse(gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_golesObjetivoIndex]);
        Constants_Handball.n_limit_goles = golesObjetivoValue;
        Constants_Handball.n_limit_goles_index = golesObjetivoIndex;
        switch (golesObjetivoIndex)
        {
            case 0:
                Goles5.isOn = true;
                break;
            case 1:
                Goles10.isOn = true;
                break;
            case 2:
                Goles15.isOn = true;
                break;
            case 3:
                Goles20.isOn = true;
                break;
        }

        Constants_Handball.idEjercicio = int.Parse(gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_idEjercicio]);

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_handballadapted] == "true")
        {
            TogggleAdaptedHandball.isOn = true;
            Constants_Handball.HandballType = HandballType.AdaptedHandball;
        }
        else
        {
            TogggleAdaptedHandball.isOn = false;
            Constants_Handball.HandballType = HandballType.Handball;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_desplazamientoCanon] == "true")
        {
            ToggleDesplazamientoCanon.isOn = true;
            Constants_Handball.isActiveDesplazamientoCanon = true;
        }
        else
        {
            ToggleDesplazamientoCanon.isOn = false;
            Constants_Handball.isActiveDesplazamientoCanon = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_rotacionPelota] == "true")
        {
            ToggleRotacionPelota.isOn = true;
            Constants_Handball.isActiveRotacionPelota = true;
        }
        else
        {
            ToggleRotacionPelota.isOn = false;
            Constants_Handball.isActiveRotacionPelota = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_curvaLanzamiento] == "true")
        {
            ToggleCurvaLanzamiento.isOn = true;
            Constants_Handball.isActiveCurvaLanzamiento = true;
        }
        else
        {
            ToggleCurvaLanzamiento.isOn = false;
            Constants_Handball.isActiveCurvaLanzamiento = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_distintosTiposPelotas] == "true")
        {
            ToggleDistintoTamanoPelota.isOn = true;
            Constants_Handball.isActiveDistintosTiposPelotas = true;
        }
        else
        {
            ToggleDistintoTamanoPelota.isOn = false;
            Constants_Handball.isActiveDistintosTiposPelotas = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonCentral] == "true")
        {
            ToggleCentral.isOn = true;
            Constants_Handball.centralCanonActivated = true;
        }
        else
        {
            ToggleCentral.isOn = false;
            Constants_Handball.centralCanonActivated = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonPivote] == "true")
        {
            TogglePivote.isOn = true;
            Constants_Handball.pivotCanonActivated = true;
        }
        else
        {
            TogglePivote.isOn = false;
            Constants_Handball.pivotCanonActivated = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonLatIzq] == "true")
        {
            ToggleLatIzq.isOn = true;
            Constants_Handball.latIzqCanonActivated = true;
        }
        else
        {
            ToggleLatIzq.isOn = false;
            Constants_Handball.latIzqCanonActivated = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonLatDer] == "true")
        {
            ToggleLatDer.isOn = true;
            Constants_Handball.latDerCanonActivated = true;
        }
        else
        {
            ToggleLatDer.isOn = false;
            Constants_Handball.latDerCanonActivated = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonExtIzq]  == "true"){
            ToggleExtIzq.isOn = true;
            Constants_Handball.extIzqCanonActivated = true;
        }
        else
        {
            ToggleExtIzq.isOn = false;
            Constants_Handball.extIzqCanonActivated = false;

        }

        if (gameConfig.gameConfiguration.gameOptions[KeysConfiguration.k_canonExtDer] == "true"){
            ToggleExtDer.isOn = true;
            Constants_Handball.extDerCanonActivated = true;
        }
        else
        {
            ToggleExtDer.isOn = false;
            Constants_Handball.extDerCanonActivated = false;
        }
    }
}