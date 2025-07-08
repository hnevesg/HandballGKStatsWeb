using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static void reinitProgressiveValuesIfNeccesary()
    {
        if (Constants_Handball.Difficulty == Difficulty.Progressive1)
        {
            Constants_Handball.velocidadMinimaModificadaProgresiva = Constants_Handball.velocidadMinimaInicialProgresiva;
            Constants_Handball.velocidadMaximaModificadaProgresiva = Constants_Handball.velocidadMaximaInicialProgresiva;
            Constants_Handball.bolasALanzarModificadaProgresiva = Constants_Handball.bolasALanzarInicialProgresiva;
            Constants_Handball.frecuencaLanzamientoModificadaProgresiva = Constants_Handball.frecuencaLanzamientoInicialProgresiva;
            Constants_Handball.efectoLanzamientoMinimoModificadaProgresiva = Constants_Handball.efectoLanzamientoMinimoInicialProgresiva;
            Constants_Handball.efectoLanzamientoMaximoModificadaProgresiva = Constants_Handball.efectoLanzamientoMaximoInicialProgresiva;

        }
        if (Constants_Handball.Difficulty == Difficulty.Progressive2)
        {
            Constants_Handball.velocidadMinimaModificadaProgresiva2 = Constants_Handball.velocidadMinimaInicialProgresiva2;
            Constants_Handball.velocidadMaximaModificadaProgresiva2 = Constants_Handball.velocidadMaximaInicialProgresiva2;
            Constants_Handball.bolasALanzarModificadaProgresiva2 = Constants_Handball.bolasALanzarInicialProgresiva2;
            Constants_Handball.frecuencaLanzamientoModificadaProgresiva2 = Constants_Handball.frecuencaLanzamientoInicialProgresiva2;
            Constants_Handball.efectoLanzamientoMinimoModificadaProgresiva2 = Constants_Handball.efectoLanzamientoMinimoInicialProgresiva2;
            Constants_Handball.efectoLanzamientoMaximoModificadaProgresiva2 = Constants_Handball.efectoLanzamientoMaximoInicialProgresiva2;

        }
    }
    public static void reinitHistorialValues()
    {
        HistoricalRegistry.tipoBola.Clear();
        HistoricalRegistry.puntoInicialLanzamiento.Clear();
        HistoricalRegistry.id_puntoInicialLanzamiento.Clear();
        HistoricalRegistry.puntoFinalLanzamiento.Clear();
        HistoricalRegistry.id_puntoFinalLanzamiento.Clear();
        HistoricalRegistry.paradoNoParado.Clear();
        HistoricalRegistry.manoDeLaParada.Clear();
        HistoricalRegistry.tiempoDisparo.Clear();
        HistoricalRegistry.tiempoParadaOGol.Clear();
        HistoricalRegistry.idBola.Clear();
        HistoricalRegistry.posicionesBola.Clear();
        HistoricalRegistry.tiemposReaccion.Clear();
    }
}
