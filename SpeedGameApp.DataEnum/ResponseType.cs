namespace SpeedGameApp.DataEnum;

/// <summary>
///     Représente les différents types de réponses possibles dans l'application.
/// </summary>
public enum ResponseType
{
    /// <summary>
    ///     Aucun type de réponse spécifié.
    /// </summary>
    None,

    /// <summary>
    ///     Réponse de type buzzer (premier à répondre).
    /// </summary>
    Buzzer,

    /// <summary>
    ///     Réponse sous forme de proposition libre.
    /// </summary>
    Proposition,

    /// <summary>
    ///     Réponse sous forme de QCM (question à choix multiples).
    /// </summary>
    Qcm,
}
