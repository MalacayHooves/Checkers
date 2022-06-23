using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Checkers
{
    public interface IWritingInfo
    {
        ChipCondition Condition { get; set; }
        string ChipNameInfo { get;}
        string CellNameInfo { get;}
    }
    public class Observer : MonoBehaviour, IObserver<Player>
    {
        [Tooltip("check if you are not going to play but watch"), SerializeField] private bool _isPlaying;
        //[SerializeField] private bool _isWriting;



        private static string path = Environment.CurrentDirectory + @"\Assets\Resources\Scripts\CheckersHistory.txt";
        private IDisposable _unsubscriber;

        private void OnEnable()
        {
            Player.OnObserverWrite += WriteInfo;
            if (!_isPlaying)
                File.WriteAllText(path, string.Empty);
            else
                PlayInfo();
        }

        private void OnDisable()
        {
            Player.OnObserverWrite -= WriteInfo;
        }

        public void OnCompleted()
        {
            print("The Observer has completed writing info.");
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Player player)
        {
            throw new NotImplementedException();
        }

        public virtual void Subscribe(IObservable<Player> player)
        {
            if (player != null)
                _unsubscriber = player.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            _unsubscriber.Dispose();
        }

        private void Awake()
        {
            
        }

        static async Task WriteInfo(ColorType color, string chipInfo, ChipCondition chipcondition, string cellInfo)
        {
                using (StreamWriter stream = new StreamWriter(path, true))
                {
                    await Task.Run(() =>
                    {
                        stream.WriteLine("{0} chip {1} was {2} at {3}", color, chipInfo, chipcondition, cellInfo);
                      
                    });
                    //await stream.WriteAllLinesAsync(write);
                    //stream.Write("{0} chip was {1} at {2}", color, chipcondition, cellInfo);
                    //stream.WriteLine();
                    //stream.WriteLine("Started");
                }
        }

        private void PlayInfo()
        {

        }


    }

    public enum ChipCondition
    {
        Selected,
        Moved,
        Destroyed
    }
}
