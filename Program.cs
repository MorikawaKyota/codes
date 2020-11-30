using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pronto_soccorso_OOP
{
    class Gestione
    {
        static void Main(string[] args)
        {
            Paziente pazienti = new Paziente();
            OrologioTempo Tempo = new OrologioTempo();
            bool endProgram = false;//variabile bool per terminare il programma
            do
            {
                Console.WriteLine("Pazienti in attesa: {1}                                                {0}:00", Tempo.MostraOrologio,pazienti.NumeroPazienti);//interfaccia utente
                Console.WriteLine("Gestione attività del pronto soccorso\n" +
                    "1 Inserire un paziente nella coda d'attesa\n" +
                    "2 Il tempo passa\n" +
                    "3 Chiamare pazienti in ambulatori\n" +
                    "4 Chiudere il programma");
                string choise = Console.ReadLine();
                switch (choise)
                {
                    case "1":
                        {
                            pazienti.Push();
                            break;
                        }
                    case "2":
                        {
                            Tempo.TempoScorre();
                            pazienti.IncPriorità();
                            break;
                        }
                    case "3":
                        {
                            pazienti.Pop();
                            break;
                        }
                    case "4":
                        {
                            endProgram = true;
                            break;
                        }
                }
            }
            while (endProgram == false);
        }
        public class OrologioTempo
        {
            public int MostraOrologio
            { get { return orologio; } }            //quando si usa MostraOrologio, ritorna l'ora

            //attributi mai public
            protected int orologio = 08;
            //metodo per far "trascorrere" il tempo
            public void TempoScorre()
            {
                orologio++;
                if (orologio == 24)
                    orologio = 00;
            }
        }
        public class Paziente
        {
            struct PazientiInCoda
            {
                internal string nome, cognome, dataNascita, sesso, codiceIdentificativo;
                internal int priorità;
            }

            protected int numeroPazienti = 0;//numero dei pazienti in attesa
            public int NumeroPazienti
            {
                get { return numeroPazienti; }
            }
            //matrice di dimensione 4x1000 (4 le situazioni cliniche, dal più grave al meno grave; 1000 il numero massimo dei pazienti per ogni situazione clinica
            PazientiInCoda[,] coda = new PazientiInCoda[4, 1000];

            //array contenente gli indici dell'ultimo elemento registrato di ogni riga; -1 significa che non c'è elemento registrato nella rispettiva riga
            //inoltre ha anche la funzione di indicare la colonna della matrice "coda" in cui si trova/inserisce un elemento
            //il valore massimo di ogni indice è 999
            protected int[] tails = new int[4] { -1, -1, -1, -1 };

            private string GenerazioneID(int riga, int colonna)
            {
                //l'ID è formato da 13 caratteri, 3 dal cognome, 3 dal nome, 2 dall'anno di nascita, 1 dal genere, 4 dal giorno e mese di nascita
                string id = "";
                coda[riga, colonna].cognome = coda[riga, colonna].cognome.ToUpper();
                for (int i = 0; i < coda[riga, colonna].cognome.Length && id.Length < 3; i++)
                {
                    if (coda[riga, colonna].cognome[i] != 'A' || coda[riga, colonna].cognome[i] != 'E' || coda[riga, colonna].cognome[i] != 'I' || coda[riga, colonna].cognome[i] != 'O' || coda[riga, colonna].cognome[i] != 'U')
                    {
                        id += coda[riga, colonna].cognome[i];
                    }
                }

                for (int i = 0; i < coda[riga, colonna].cognome.Length && id.Length < 3; i++)
                {
                    if (coda[riga, colonna].cognome[i] == 'A' || coda[riga, colonna].cognome[i] == 'E' || coda[riga, colonna].cognome[i] == 'I' || coda[riga, colonna].cognome[i] == 'O' || coda[riga, colonna].cognome[i] == 'U')
                    {
                        id += coda[riga, colonna].cognome[i];
                    }
                }

                while (id.Length < 3)
                {
                    id += 'X';
                }

                coda[riga, colonna].nome = coda[riga, colonna].nome.ToUpper();
                for (int i = 0; i < coda[riga, colonna].nome.Length && id.Length < 6; i++)
                {
                    if (coda[riga, colonna].nome[i] != 'A' || coda[riga, colonna].nome[i] != 'E' || coda[riga, colonna].nome[i] != 'I' || coda[riga, colonna].nome[i] != 'O' || coda[riga, colonna].nome[i] != 'U')
                    {
                        id += coda[riga, colonna].nome[i];
                    }
                }

                for (int i = 0; i < coda[riga, colonna].nome.Length && id.Length < 6; i++)
                {
                    if (coda[riga, colonna].nome[i] == 'A' || coda[riga, colonna].nome[i] == 'E' || coda[riga, colonna].nome[i] == 'I' || coda[riga, colonna].nome[i] == 'O' || coda[riga, colonna].nome[i] == 'U')
                    {
                        id += coda[riga, colonna].nome[i];
                    }
                }

                while (id.Length < 6)
                {
                    id += 'X';
                }

                id += coda[riga, colonna].dataNascita.Substring(6, 2);

                coda[riga, colonna].sesso = coda[riga, colonna].sesso.ToUpper();
                id = coda[riga, colonna].sesso.Substring(0, 1);

                id += coda[riga, colonna].dataNascita.Substring(0, 4);
                return id;
            }
            public void Push()
            {
                Console.WriteLine("Indicare la situazione clinica del paziente:\n1-Molto grave\n2-Grave\n3-Lievemente grave\n4-Non grave");
                int riga = Int32.Parse(Console.ReadLine()) - 1;//la variabile "riga" indica la riga della matrice "coda" in cui inserire l'elemento; si sottrae 1 perchè gli indici partono da 0
                if (tails[riga] + 1 < 1000)
                {
                    tails[riga]++;
                    //acquisizione dei dati del paziente
                    Console.WriteLine("Digitare il nome del paziente.");
                    coda[riga, tails[riga]].nome = Console.ReadLine();
                    Console.WriteLine("Digitare il cognome del paziente.");
                    coda[riga, tails[riga]].cognome = Console.ReadLine();
                    Console.WriteLine("Digitare la data di nascita del paziente secondo lo schema ggmmnnnn.");
                    coda[riga, tails[riga]].dataNascita = Console.ReadLine();
                    Console.WriteLine("Scegliere il genere del paziente, digitare m maschio, f per femmina");
                    coda[riga, tails[riga]].sesso = Console.ReadLine();
                    coda[riga, tails[riga]].codiceIdentificativo = GenerazioneID(riga, tails[riga]);
                    coda[riga, tails[riga]].priorità = 0;
                    numeroPazienti++;
                }
                else
                {
                    Console.WriteLine("Non ci sono più posti liberi! Contattare altri ospedali!");
                }
            }
            public void Pop()
            {
                Random rdm = new Random();
                //ambulatoriLiberi indica il numero di ambulatori liberi che possono ricevere un paziente; il valore massimo è 7
                int ambulatoriLiberi = rdm.Next(0, 7);

                for (int i = 0; i < ambulatoriLiberi; i++)
                {
                    for (int riga = 0; riga < 4; riga++)
                    {
                        if (tails[riga] >= 0)//verificare se la riga attuale ha elementi registrati o meno
                        {
                            for (int colonna = 0; colonna <= tails[riga]; colonna++)//spostamento orizzontale a sinistra degli elementi
                            {
                                if (colonna == tails[riga])
                                {//si cancella il contenuto dell'ultimo elemento per evitare di avere 2 copie identiche di esso dopo lo spostamento
                                    coda[riga, colonna].nome = null;
                                    coda[riga, colonna].cognome = null;
                                    coda[riga, colonna].dataNascita = null;
                                    coda[riga, colonna].sesso = null;
                                    coda[riga, colonna].priorità = 0;
                                    coda[riga, colonna].codiceIdentificativo = null;
                                }
                                else
                                {//l'elemento attuale copia il contenuto di quello successivo e così via
                                    coda[riga, colonna].nome = coda[riga, colonna + 1].nome;
                                    coda[riga, colonna].cognome = coda[riga, colonna + 1].cognome;
                                    coda[riga, colonna].dataNascita = coda[riga, colonna + 1].dataNascita;
                                    coda[riga, colonna].sesso = coda[riga, colonna + 1].sesso;
                                    coda[riga, colonna].priorità = coda[riga, colonna + 1].priorità;
                                    coda[riga, colonna].codiceIdentificativo = coda[riga, colonna + 1].codiceIdentificativo;
                                }
                            }
                            tails[riga]--;
                            numeroPazienti--;
                            break;//terminato lo spostamento della riga si interrompe il ciclo per riempire il successivo ambulatorio 
                        }
                    }
                }
            }
            public void IncPriorità()
            {
                for (int riga = 0; riga < 4; riga++)
                {
                    for (int colonna = 0; colonna <= tails[riga]; colonna++)
                    {
                        //controllare se la priorità del primo elemento ha raggiunto un certo numero e se la riga precedente è ancora libera o meno
                        if (riga != 0 && coda[riga, colonna].priorità + 1 == 10 && tails[riga - 1] + 1 < coda.Length)
                        {
                            //se la condizione è verificata, si sposta il primo elemento della riga attuale in quella precedente collocandolo alla fine
                            tails[riga - 1]++;
                            coda[riga - 1, tails[riga - 1]].nome = coda[riga, colonna].nome;
                            coda[riga - 1, tails[riga - 1]].cognome = coda[riga, colonna].cognome;
                            coda[riga - 1, tails[riga - 1]].dataNascita = coda[riga, colonna].dataNascita;
                            coda[riga - 1, tails[riga - 1]].sesso = coda[riga, colonna].sesso;
                            coda[riga - 1, tails[riga - 1]].priorità = coda[riga, colonna].priorità;
                            coda[riga - 1, tails[riga - 1]].codiceIdentificativo = coda[riga, colonna].codiceIdentificativo;
                            for (int i = 0; i <= tails[riga]; i++)//poi si ricompatta la riga
                            {
                                if (i == tails[riga])
                                {
                                    coda[riga, i].nome = null;
                                    coda[riga, i].cognome = null;
                                    coda[riga, i].dataNascita = null;
                                    coda[riga, i].sesso = null;
                                    coda[riga, i].priorità = 0;
                                    coda[riga, i].codiceIdentificativo = null;
                                }
                                else
                                {
                                    coda[riga, i].nome = coda[riga, i + 1].nome;
                                    coda[riga, i].cognome = coda[riga, i + 1].cognome;
                                    coda[riga, i].dataNascita = coda[riga, i + 1].dataNascita;
                                    coda[riga, i].sesso = coda[riga, i + 1].sesso;
                                    coda[riga, i].priorità = coda[riga, i + 1].priorità;
                                    coda[riga, i].codiceIdentificativo = coda[riga, i + 1].codiceIdentificativo;
                                }
                            }
                            tails[riga]--;
                        }
                        else//se la condizione non è verificata, si imcrementano semplicemente le priorità
                        {
                            coda[riga, colonna].priorità++;
                        }
                    }
                }
            }
        }
    }
}
