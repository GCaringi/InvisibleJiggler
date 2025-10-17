# Mouse Jiggler .NET

Un'applicazione console .NET leggera e portatile che simula movimenti del mouse per prevenire la sospensione del sistema e il blocco schermo. Perfetta per ambienti dove non è possibile installare programmi esterni o serve una soluzione a footprint minimo.

## ✨ Caratteristiche

- **Nessuna installazione richiesta**: Funziona come semplice applicazione console senza permessi amministrativi
- **Ultra-leggero**: Nessuna dipendenza esterna, solo chiamate alle API Windows native tramite P/Invoke
- **Footprint di memoria minimo**: Architettura console che mantiene l'uso delle risorse estremamente basso
- **Movimenti naturali**: Spostamenti casuali in 4 direzioni con distanze variabili
- **Previene la sospensione**: Usa `SetThreadExecutionState` per impedire standby e timeout dello schermo
- **Portabile**: Eseguibile singolo che può essere eseguito da qualsiasi cartella senza installazione
- **Codice pulito**: Struttura modulare con separazione delle responsabilità

## 🎯 Caso d'Uso

Questo progetto nasce dall'esigenza di avere un mouse jiggler su computer dove:
- L'installazione di programmi esterni è limitata o vietata
- Non sono disponibili privilegi amministrativi
- È richiesta una soluzione portatile
- Il consumo minimo di risorse è essenziale

## 🚀 Requisiti

- Windows 10/11
- Runtime .NET 10.0

## 📦 Installazione

1. Clona la repository:
git clone https://github.com/tuousername/mouse-jiggler-dotnet.git
cd mouse-jiggler-dotnet

2. Compila il progetto:
dotnet build -c Release

3. Esegui l'applicazione:
dotnet run

## 🔧 Come Funziona

1. **SetThreadExecutionState**: Impedisce al sistema di entrare in standby e allo schermo di spegnersi usando i flag `ES_CONTINUOUS`, `ES_SYSTEM_REQUIRED` e `ES_DISPLAY_REQUIRED`

2. **SendInput**: Simula movimenti reali del mouse che vengono riconosciuti da Windows e dalle applicazioni come attività utente

3. **Loop intelligente**: Movimenti casuali ogni 3-4 minuti con direzioni e distanze variabili

## ⚙️ Personalizzazione

Puoi modificare facilmente:

- **Intervallo tra movimenti**: Modifica `Thread.Sleep(rnd.Next(180000, 240000))` in `MouseJiggler.cs`
- **Distanza movimento**: Cambia `_random.Next(50, 101)` per range di pixel diversi
- **Comportamento sospensione**: Rimuovi flag in `Constants.cs`

## 📝 Note

- L'applicazione non richiede privilegi di amministratore
- Il movimento torna sempre alla posizione originale
- Non interferisce con l'uso normale del PC
- All'uscita (Ctrl+C) ripristina automaticamente le impostazioni di risparmio energetico

## 🤝 Contributi

I contributi sono benvenuti! Sentiti libero di aprire issue o pull request.

## 📄 Licenza

Questo progetto è rilasciato sotto licenza MIT. Vedi il file LICENSE per i dettagli.

## ⚠️ Disclaimer

Questo software è fornito "così com'è" per scopi educativi e personali. Usa a tuo rischio e responsabilità.