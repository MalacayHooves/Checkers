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
        [SerializeField] private bool _isPlaying;
        //[SerializeField] private bool _isWriting;

     

        private IDisposable _unsubscriber;
        private static string path = Environment.CurrentDirectory + @"\Assets\Resources\Scripts\CheckersHistory.txt";

        private void OnEnable()
        {
            Player.OnTurnEnd += WriteInfo;
            File.WriteAllText(path, string.Empty);
        }

        private void OnDisable()
        {
            Player.OnTurnEnd -= WriteInfo;
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

        static async Task WriteInfo(ColorType color, ChipCondition chipcondition, string cellInfo)
        {
                using (StreamWriter stream = new StreamWriter(path, true))
                {
                    await Task.Run(() =>
                    {
                        stream.WriteLine("{0} chip was {1} at {2}", color, chipcondition, cellInfo);
                      
                    });
                    //await stream.WriteAllLinesAsync(write);
                    //stream.Write("{0} chip was {1} at {2}", color, chipcondition, cellInfo);
                    //stream.WriteLine();
                    //stream.WriteLine("Started");
                }
        }


    }

    public enum ChipCondition
    {
        Selected,
        Moved,
        Destroyed
    }
}
