# Architecture et Conception - SpeedGameApp

## Vue d'ensemble

SpeedGameApp est une application développée en C# suivant une architecture multicouche qui sépare clairement les responsabilités. Le projet a été créé le 29 décembre 2022 et utilise une approche modulaire pour organiser le code.

## Structure du Projet

### Organisation des Couches

L'application suit une architecture en couches classique avec les modules suivants :

#### 1. **SpeedGameApp** (Couche Présentation)
- Point d'entrée principal de l'application
- Interface utilisateur et logique de présentation
- Gestion des interactions utilisateur

#### 2. **SpeedGameApp.Business** (Couche Métier)
- Logique métier de l'application
- Règles de gestion et traitements
- Services et orchestration des opérations

#### 3. **SpeedGameApp.DataAccessLayer** (Couche d'Accès aux Données)
- Gestion de l'accès aux données
- Interactions avec la base de données
- Modèles de données et repositories

#### 4. **SpeedGameApp.DataEnum** (Énumérations)
- Définitions des énumérations partagées
- Constantes et types de données statiques
- Valeurs de référence utilisées dans l'application

## Architecture Technique

### Configuration et Standards

- **Gestionnaire de paquets** : Utilisation de Central Package Management via `Directory.Packages.props`
- **Standards de code** : Configuration StyleCop via `stylecop.json` pour maintenir la qualité du code
- **Configuration de l'éditeur** : Fichier `.editorconfig` pour assurer la cohérence du formatage

### Base de Données

- Dossier `sql/` contenant les scripts de base de données
- Scripts de création, migration et données de référence

### Intégration Continue

- Configuration GitHub Actions dans le dossier `.github/`
- Automatisation des builds et tests

## Principes de Conception

### Séparation des Responsabilités
- **Présentation** : Interface utilisateur isolée dans le module principal
- **Métier** : Logique applicative centralisée dans le module Business
- **Données** : Accès aux données encapsulé dans le DataAccessLayer
- **Types** : Énumérations partagées dans un module dédié

### Avantages de cette Architecture

1. **Maintenabilité** : Séparation claire des couches facilite la maintenance
2. **Testabilité** : Chaque couche peut être testée indépendamment
3. **Évolutivité** : Modifications isolées dans chaque couche
4. **Réutilisabilité** : Composants métier réutilisables
5. **Standards** : Application de règles de codage cohérentes

## Gestion des Dépendances

Le projet utilise le système de gestion centralisée des packages .NET avec :
- `Directory.Packages.props` pour la gestion des versions
- Configuration des dépendances au niveau solution

## Environnement de Développement

- **Langage** : C#
- **Type de projet** : Application .NET
- **Gestion de version** : Git avec GitHub
- **Standards** : StyleCop pour l'analyse statique du code

Cette architecture modulaire permet une séparation nette des responsabilités et facilite la maintenance et l'évolution de l'application SpeedGameApp.