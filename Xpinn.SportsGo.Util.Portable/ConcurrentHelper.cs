using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Xpinn.SportsGo.Util.Portable
{
    public class ConcurrentHelper<TToProcess, TToConsume>
    {
        // Se debe llamar al metodo CompleteAdding() al terminar de producir trabajo, para que el metodo de Consumir deje de esperar por mas
        BlockingCollection<TToConsume> _results = new BlockingCollection<TToConsume>();

        /// <summary>
        /// Este metodo produce trabajo para ser consumido por el ConsumeWork metodo añadiendolo a la BlockingCollection
        /// Puedes pasar un Func (UN METODO) que no necesite parametros, el mismo procesa los datos que estan dentro de el, el metodo debe retornar un IEnumerable<TToConsume>
        /// Recordar Siempre llamar este metodo con un Task.Run o Task.Factory.StartNew para evitar bloqueo de Threads
        /// </summary>
        /// <param name="workToProduce">El metodo que va a procesar su propio trabajo, no necesita parametros</param>
        /// <returns>true si todo fue exitoso, false de lo contrario</returns>
        public bool ProduceWork(Func<IEnumerable<TToConsume>> workToProduce)
        {
            if (workToProduce == null) throw new ArgumentNullException("Producidor de trabajo no puede ser nulo!.");
            try
            {
                foreach (var work in workToProduce())
                {
                    _results.Add(work);
                }

                _results.CompleteAdding();
                return true;
            }
            catch (Exception)
            {
                _results.CompleteAdding();
                return false;
            }
        }


        /// <summary>
        /// Este metodo produce trabajo para ser consumido por el ConsumeWork metodo añadiendolo a la BlockingCollection
        /// Puedes pasar un Func (UN METODO) que necesite un parametro, ese parametro es lo que se va a procesar, debe retornar un IEnumerable<TToConsume>
        /// Le pasas lo que vas a procesar en el primer parametro, y el metodo para procesarlo en el segundo
        /// Recordar Siempre llamar este metodo con un Task.Run o Task.Factory.StartNew para evitar bloqueo de Threads
        /// </summary>
        /// <param name="toProcess">Lo que vas a procesar</param>
        /// <param name="workToProduce">El metodo que va a procesar</param>
        /// <returns>true si todo fue exitoso, false de lo contrario</returns>
        public bool ProduceWork(TToProcess toProcess, Func<TToProcess, IEnumerable<TToConsume>> workToProduce)
        {
            if (workToProduce == null) throw new ArgumentNullException("Producidor de trabajo no puede ser nulo!.");

            try
            {
                foreach (var work in workToProduce(toProcess))
                {
                    _results.Add(work);
                }

                _results.CompleteAdding();
                return true;
            }
            catch (Exception)
            {
                _results.CompleteAdding();
                return false;
            }
        }

        /// <summary>
        /// Este metodo consume el trabajo que halla dejado el ProduceWork metodo en la BlockingCollection
        /// El Blocking Collection interno bloquea el thread en el que se ejecuta esperando por trabajo que consumir
        /// Puedes pasar un Action (UN METODO) que no retorne nada y reciba como parametro un IEnumerable<TToConsume>
        /// Recordar Siempre llamar este metodo con un Task.Run o Task.Factory.StartNew para evitar bloqueo de Threads
        /// </summary>
        /// <param name="consumerOfWork">Metodo que procesa el trabajo</param>
        ///  <returns>true si todo fue exitoso, false de lo contrario</returns>
        public bool ConsumeWork(Action<TToConsume> consumerOfWork)
        {
            if (consumerOfWork == null) throw new ArgumentNullException("Consumidor de trabajo no puede ser nulo!.");

            try
            {
                while (!_results.IsCompleted)
                {
                    TToConsume work = _results.Take();

                    consumerOfWork(work);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
