# Avancements du Projet "Flappy Gun"

Ce document résume les progrès réalisés sur le projet Flappy Gun.

### 1. Conception et Initialisation du Projet

*   **Analyse du besoin :** Le projet a été initié sur la base du document de conception (`README.md`) pour définir les mécaniques de jeu.
*   **Création de la structure :** Une base de code complète a été générée en C# avec des scripts dédiés pour chaque fonctionnalité :
    *   `PlayerController` pour gérer le joueur.
    *   `GameManager` pour la logique globale et les états de jeu (menu, jeu, fin).
    *   `EnemyController` et `EnemySpawner` pour le comportement et l'apparition des ennemis.
    *   `UIManager` pour piloter l'interface utilisateur.
    *   Des scripts pour les objets annexes comme les `Bullet` (projectiles) et les `VFXSelfDestruct` (effets visuels temporaires).

### 2. Implémentation de la Boucle de Gameplay Principale

*   **Le Joueur :** Un joueur a été créé, subissant la gravité et capable de se propulser en tirant.
*   **Le Tir :** Cliquer sur l'écran fait tirer une balle et applique une force de recul au joueur.
*   **La Visée :** Le joueur s'oriente désormais dynamiquement pour viser la position de la souris en temps réel.
*   **Les Ennemis :** Des ennemis apparaissent périodiquement des deux côtés de l'écran et se déplacent en effectuant des "bonds", comme défini dans le concept.
*   **Le Score :** Le score du joueur augmente à chaque fois qu'un ennemi est détruit.
*   **Conditions de Victoire/Défaite :** La partie se termine si le joueur entre en collision avec le sol ou un ennemi. Il n'y a pas de condition de victoire finale, l'objectif est de survivre le plus longtemps possible.

### 3. Interface Utilisateur et Feedback Visuel

*   **Système de Menus :** Une interface utilisateur fonctionnelle a été mise en place, incluant un écran de démarrage et un écran de fin de partie qui affiche le score final avec une option pour "Rejouer".
*   **Affichage en jeu (HUD) :** Le score actuel est affiché et mis à jour en temps réel pendant la partie.
*   **Effets Visuels (VFX) :** Pour améliorer le feedback visuel, des effets simples ont été ajoutés :
    *   Un "Muzzle Flash" (flash de tir) apparaît à la bouche du canon à chaque tir.
    *   Un "Death Effect" (effet d'explosion) apparaît lorsqu'un ennemi est détruit. 