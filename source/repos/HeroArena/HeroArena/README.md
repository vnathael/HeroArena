# Hero Arena

Application de combat tour par tour entre héros, développée en **C# WPF** avec **Entity Framework Core** et une base de données **SQL Server**.

---

## Concept

Hero Arena est un jeu où le joueur choisit un héros parmis 3 (Larry, Franck, Tung tung) possédant chacun 4 sorts uniques, et affronte un ennemi généré automatiquement avec des statistiques améliorées dans une arène de combat tour par tour.

---

## Prérequis

Visual Studio Community 2022 
.NET 8.0
SQL Server 2022
SQL Server Management Studio 2022 

---

## Packages NuGet

| `Microsoft.EntityFrameworkCore.SqlServer` | Connexion et requêtes SQL Server via EF Core |
| `Microsoft.EntityFrameworkCore.Tools` | Outils EF Core (migrations, scaffolding) |
| `BCrypt.Net-Next` | Hashage sécurisé des mots de passe |

---

## Installation de la base de données

### 1. Ouvrir SQL Server Management Studio (SSMS)

### 2. Exécuter le script SQL suivant pour créer la base et les tables :

```sql
-- Création de la base de données
CREATE DATABASE ExerciceHero;
GO

USE ExerciceHero;
GO

-- Table Login
CREATE TABLE Login (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL
);

-- Table Player
CREATE TABLE Player (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    LoginID INT,
    FOREIGN KEY (LoginID) REFERENCES Login(ID)
);

-- Table Hero
CREATE TABLE Hero (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Health INT NOT NULL,
    ImageURL NVARCHAR(255) NULL
);

-- Table Spell
CREATE TABLE Spell (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL,
    Damage INT NOT NULL,
    Description NVARCHAR(MAX)
);

-- Table PlayerHero (relation Player <-> Hero)
CREATE TABLE PlayerHero (
    PlayerID INT NOT NULL,
    HeroID INT NOT NULL,
    PRIMARY KEY (PlayerID, HeroID),
    FOREIGN KEY (PlayerID) REFERENCES Player(ID),
    FOREIGN KEY (HeroID) REFERENCES Hero(ID)
);

-- Table HeroSpell (relation Hero <-> Spell)
CREATE TABLE HeroSpell (
    HeroID INT NOT NULL,
    SpellID INT NOT NULL,
    PRIMARY KEY (HeroID, SpellID),
    FOREIGN KEY (HeroID) REFERENCES Hero(ID),
    FOREIGN KEY (SpellID) REFERENCES Spell(ID)
);
```

---

## Lancement du projet

### 1. Cloner le repository

```bash
git clone https://github.com/TON_USERNAME/HeroArena.git
```

### 2. Ouvrir le projet dans Visual Studio 2022

Double-cliquer sur `HeroArena.sln`.

### 3. Restaurer les packages NuGet

Visual Studio le fait automatiquement au premier lancement. Sinon :
**Outils → Gestionnaire de packages NuGet → Restaurer les packages NuGet**

### 4. Lancer l'application

Appuyer sur **F5** ou cliquer sur le bouton ▶ **HeroArena**.

---

## Configuration de la base de données

### 1. Trouver votre nom de serveur SQL

Dans SSMS, le nom du serveur est visible dans l'explorateur d'objets en haut à gauche.  
Il ressemble à : `PC\SQLEXPRESS` ou `localhost`.

### 2. Configurer la chaîne de connexion dans l'application

Au démarrage, cliquer sur **Paramètres BDD** et entrer votre chaîne de connexion :

```
Server=PC\SQLEXPRESS;Database=ExerciceHero;Trusted_Connection=True;TrustServerCertificate=True;
```

Remplacer `PC\SQLEXPRESS` par le nom de votre serveur.

### 3. Initialiser les données par défaut

Cliquer sur **Initialiser les données par défaut**.  
Cela insère en base : les héros, leurs sorts, et un compte administrateur.

---

## 👤 Compte par défaut

| Champ | Valeur |
|-------|--------|
| Username | `admin` |
| Password | `admin123` |

> Il est également possible de créer son propre compte via le bouton **Créer un compte** sur l'écran de connexion.

---

## Fonctionnalités

### Écran de connexion
- Connexion avec nom d'utilisateur et mot de passe
- Mot de passe hashé en base avec **BCrypt**
- Accès aux paramètres BDD
- Création de compte

### Écran d'inscription
- Création d'un compte utilisateur personnalisé
- Validation : nom d'utilisateur unique, mot de passe minimum 6 caractères, confirmation du mot de passe
- Compte enregistré en base de données

### Paramètres BDD
- Modification de la chaîne de connexion SQL Server
- Initialisation des données par défaut en un clic

### Onglet Héros
- Liste de tous les héros disponibles
- Affichage détaillé : nom, points de vie, sorts associés avec dégâts et descriptions

### Onglet Spells
- Liste complète de tous les sorts du jeu
- Affichage détaillé : nom, dégâts, description
- Filtre par héros

### Onglet Combat
- Sélection du héros joueur
- Combat tour par tour contre un ennemi généré aléatoirement
- L'ennemi possède **+10% de HP** et **+5% de dégâts** par rapport à un héros normal
- Barres de HP visuelles pour le joueur et l'ennemi
- Utilisation des sorts pour infliger des dégâts
- L'ennemi joue automatiquement après chaque action du joueur
- Score incrémenté à chaque victoire
- Bouton pour relancer un nouveau combat

---

## 🏗 Architecture du projet (MVVM)

```
HeroArena/
├── Models/                  
│   ├── Login.cs
│   ├── Player.cs
│   ├── Hero.cs
│   ├── Spell.cs
│   ├── PlayerHero.cs
│   └── HeroSpell.cs
│
├── ViewModels/              
│   ├── ViewModelBase.cs
│   ├── RelayCommand.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── HeroViewModel.cs
│   ├── SpellViewModel.cs
│   └── CombatViewModel.cs
│
├── Views/                  
│   ├── LoginView.xaml
│   ├── RegisterView.xaml
│   ├── SettingsView.xaml
│   ├── MainWindow.xaml
│   ├── HeroView.xaml
│   ├── SpellView.xaml
│   └── CombatView.xaml
│
├── Data/                    
│   └── AppDbContext.cs
│
├── Converters/              
│   ├── NullToVisibilityConverter.cs
│   └── BoolToVisibilityConverter.cs
│
└── Resources/               
    └── AppResources.xaml
```

---

## Réinitialiser les données

Si vous souhaitez repartir de zéro, exécutez ce script dans SSMS :

```sql
USE ExerciceHero;

DELETE FROM HeroSpell;
DELETE FROM PlayerHero;
DELETE FROM Player;
DELETE FROM Login;
DELETE FROM Spell;
DELETE FROM Hero;

DBCC CHECKIDENT ('Hero', RESEED, 0);
DBCC CHECKIDENT ('Spell', RESEED, 0);
DBCC CHECKIDENT ('Login', RESEED, 0);
DBCC CHECKIDENT ('Player', RESEED, 0);
```

Puis relancer **"Initialiser les données par défaut"** dans l'application.
