# GSPro Mission Control

A lightweight touch-friendly companion app for **GSPro golf simulator** that lets you control key GSPro functions without touching the main PC.

Built for simulator setups where you want quick access to flyovers, UI toggles, and navigation — without breaking focus or immersion.

---

## ✨ Features

- ✅ Sends keystrokes directly to GSPro  
- ✅ Never steals focus from GSPro
- ✅ Touch-friendly UI (perfect for sim kiosks & tablets)  
- ✅ Clean, minimal “cockpit-style” layout  
- ✅ Designed for real-world simulator play  
- ✅ Works great in dedicated sim environments

---

## 🎯 Purpose

GSPro is fantastic, but interacting with it during play can be clumsy when:

- Using a touch screen  
- Standing away from the keyboard/mouse  
- Running a dedicated sim PC  
- Wanting a more “control panel” style experience

Mission Control solves that.

---

## 🖥️ Requirements

- Windows PC running GSPro  
- GSPro configured with standard keyboard shortcuts  
- .NET runtime (if not publishing self-contained)

---

## 🚀 Installation

1. Download the latest release  
2. Launch the app on the same PC as GSPro  
3. Keep GSPro as the focused window  
4. Use Mission Control as your touch control panel

No special GSPro configuration required.

---

## ⌨️ Default Controls

| Action        | Key Sent                     |
|--------------|------------------------------|
| Flyover      | `O`                          |
| Aim          | `J`                          |
| Heat Map     | `Y`                          |
| Hide Objects | `B`                          |
| Hide UI      | `H`                          |
| Camera Up    | `Arrow Up` (+100 ms delay)   |
| Camera Down  | `Arrow Down` (+100 ms delay) |
| Camera Left  | `Arrow Left` (+100 ms delay) |
| Camera Right | `Arrow Right` (+100 ms delay)|
| Tee Back     | `C`                          |
| Tee Forward  | `V`                          |
| Club Up      | `I`                          |
| Club Down    | `K`                          |
| Putter       | `U`                          |
| Score Card   | `T`                          |

You can modify key mappings in the source if needed.

---

## 🛠️ Built With

- Avalonia UI  
- .NET  
- Windows SendInput API for key injection

---

## 📌 Status

This is an early but stable version used in a real simulator environment.

More improvements and features are planned for future versions.

---

## 🤝 Contributing

Suggestions and ideas are welcome!

Open an issue if you:

- Have feature ideas  
- Find bugs  
- Want additional GSPro integrations

---

## ⚠️ Disclaimer

This project is not affiliated with or endorsed by GSPro.

Use at your own discretion.

---

## 📄 License

GNU General Public License v3.0

---

## ⛳ Why this exists

Built by a golfer for golfers.

If it improves your simulator experience, that’s a win.
