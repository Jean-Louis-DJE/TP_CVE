# TP CVE — Environnement Virtuel Collaboratif (VR/AR)

Projet pédagogique Unity mettant en œuvre un Environnement Virtuel Collaboratif (CVE) multi-plateforme, combinant un casque VR (Oculus) et un téléphone AR dans un espace partagé en réseau.

---

## Description

Ce TP explore la création d'un espace virtuel collaboratif où plusieurs utilisateurs, sur des appareils différents, interagissent en temps réel avec des objets partagés. Un PC joue le rôle d'hôte, un casque Oculus et un téléphone Android (AR) se connectent en tant que clients.

**Stack technique :**
- Moteur : Unity 6000.3.8f1
- Pipeline de rendu : Universal Render Pipeline (URP)
- Réseau : Unity Netcode for GameObjects + Unity Services Multiplayer
- VR : Oculus / Meta XR — XR Interaction Toolkit 3.x
- AR : ARFoundation + ARCore (Android)

---

## Architecture du projet

```
TP_CVE_RVRA_2/
├── Assets/
│   ├── Scripts/          # Scripts C# du projet
│   ├── Scenes/           # VRScene.unity, SampleScene.unity
│   ├── Prefabs/          # NetworkedPlayer, SharedCube, VRPlayer, ARCube
│   ├── XR/               # Configuration XR
│   └── Settings/         # Paramètres URP et XR
├── Packages/             # Dépendances Unity (manifest.json)
├── ProjectSettings/      # Configuration du projet Unity
├── Game/                 # Build PC (hôte)
└── VR_CVE_TP.apk         # Build Android (casque VR)
```

---

## Scripts

### Connexion réseau

| Script | Rôle |
|--------|------|
| `ConnectionManager.cs` | Gestion des sessions via Unity Services : création ou rejoindre une session, authentification, autorité distribuée |
| `VRConnectionManager.cs` | Connexion du casque Oculus au PC hôte (IP configurable dans l'Inspector) |
| `PhoneConnectionManager.cs` | Connexion du téléphone AR au PC hôte (même logique, port 7777 par défaut) |

### Joueur & déplacement

| Script | Rôle |
|--------|------|
| `PlayerController.cs` | Déplacement du joueur synchronisé sur le réseau (étend `NetworkTransform`) ; attache la caméra au rig de navigation à l'initialisation |

### Interaction avec les objets

| Script | Rôle |
|--------|------|
| `NetworkedXRGrabInteractable1.cs` | Objet saisissable en VR synchronisé en réseau : changement de couleur (cyan = saisissable, jaune = saisi), transfert d'ownership lors de la saisie |
| `NetworkedXRDirectInteractor.cs` | Interacteur direct VR avec synchronisation réseau |
| `XRCursorDriver.cs` | Curseur VR piloté par le ray interactor de la main droite : repositionne le curseur sur l'objet visé, spawn un objet réseau sur pression du bouton A |
| `CursorDriver.cs` | Équivalent souris pour tests en éditeur : Alt maintenu pour déplacer le curseur, P pour spawner un objet réseau |

---

## Fonctionnement de la session

1. Le **PC** lance la scène en hôte et crée la session collaborative.
2. Le **casque Oculus** (APK Android) et/ou le **téléphone AR** se connectent en tant que clients via l'IP du PC.
3. Chaque client est représenté par un `NetworkedPlayer` dont la position est synchronisée.
4. Les **objets partagés** (SharedCube) peuvent être saisis par n'importe quel participant : la saisie transfère l'ownership de l'objet au client actif, et la couleur change pour tous les participants.
5. Un joueur peut **spawner de nouveaux objets** dans l'espace partagé via le curseur VR (bouton A) ou le curseur souris (touche P).

---

## Prérequis

- Unity 6000.3.8f1 avec les modules **Android Build Support** et **XR**
- Casque Oculus/Meta compatible Android
- Téléphone Android compatible ARCore
- PC hôte et appareils clients sur le **même réseau local**
- Compte Unity (pour Unity Gaming Services)

---

## Lancer le projet

1. Ouvrir le projet dans Unity 6000.3.8f1
2. Ouvrir la scène `Assets/Scenes/VRScene.unity`
3. Renseigner l'IP du PC hôte dans les composants `VRConnectionManager` et `ARPhoneConnectionManager` via l'Inspector
4. Lancer le build PC (`File > Build and Run`) — le PC fait office d'hôte
5. Déployer `VR_CVE_TP.apk` sur le casque Oculus
6. Lancer la scène AR sur le téléphone (build Android séparé)
7. Les clients rejoignent la session via le bouton de connexion affiché à l'écran

---

## Dépendances principales

| Package | Version |
|---------|---------|
| com.unity.netcode.gameobjects | 2.9.2 |
| com.unity.services.multiplayer | 2.0.0 |
| com.unity.xr.interaction.toolkit | 3.3.1 |
| com.unity.xr.oculus | 4.5.4 |
| com.unity.xr.arfoundation | 6.3.3 |
| com.unity.xr.arcore | 6.3.3 |
| com.unity.render-pipelines.universal | 17.3.0 |

---

## Licence

Ce projet est distribué sous licence [CC0 1.0 Universal](LICENSE) — Domaine public.
