﻿using System.ServiceModel;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// The Contract for the audiosource host which will act as the server
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IAudioSourceHost
    {
        [OperationContract]
        string GetName();

        [OperationContract]
        Task ActivateAsync();

        [OperationContract]
        Task DeactivateAsync();

        [OperationContract]
        Task PlayTrackAsync();

        [OperationContract]
        Task PauseTrackAsync();

        [OperationContract]
        Task PreviousTrackAsync();

        [OperationContract]
        Task NextTrackAsync();
    }
}