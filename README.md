# Document de Conception du Jeu : Flappy Gun

**1. Concept Général**

*   **Nom du jeu :** Flappy Gun
*   **Genre :** Jeu d'arcade mobile simple (action/survie)
*   **Principe :** Le joueur contrôle un revolver qui tombe constamment à cause de la gravité. En touchant l'écran, le joueur fait tirer le revolver dans la direction du point touché. La force de recul propulse le revolver dans la direction opposée au tir, permettant de le maintenir en l'air et de viser. Le but est d'éliminer un maximum d'ennemis (des "Flappy Birds") qui traversent l'écran.

**2. Mécaniques de Jeu Principales**

*   **Contrôle du Revolver :**
    *   Le joueur touche un point (P) sur l'écran.
    *   Le revolver s'oriente instantanément pour viser le point P.
    *   Le revolver tire une balle vers P.
    *   Le revolver subit une force de recul dans la direction opposée au tir (de P vers le revolver et au-delà).
    *   La magnitude de la force de recul est constante à chaque tir.
    *   Il n'y a pas de rotation du sprite du revolver induite par le recul (torque), mais un paramètre doit être prévu pour l'ajuster ultérieurement si besoin.
*   **Gravité :**
    *   Le revolver est constamment attiré vers le bas par une force de gravité simulant une accélération (comme dans la "vraie vie").
*   **Tir :**
    *   **Cadence :** Il y a un léger délai (cooldown) entre chaque tir pour éviter les tirs trop rapprochés.
    *   **Munitions :**
        *   Le revolver a une capacité de 6 balles.
        *   Si le joueur tente de tirer sans munitions, un son de "clic à vide" est joué et aucun tir n'est effectué.
    *   **Recharge :**
        *   Des sprites de "chargeurs" apparaissent à l'écran à des positions aléatoires et à des intervalles de temps réguliers.
        *   Ces sprites de chargeurs sont fixes (ne se déplacent pas).
        *   Si le sprite du revolver entre en collision avec un sprite de chargeur, le chargeur du revolver est instantanément rempli (6 balles).
        *   Lors de la collecte, le sprite de chargeur disparaît avec un effet sonore et visuel.
*   **Mouvement Horizontal du Revolver :**
    *   Le mouvement horizontal est uniquement induit par la composante horizontale de la force de recul. Il n'y a pas de dérive horizontale passive.

**3. Ennemis ("Flappy Birds")**

*   **Apparition :**
    *   Les ennemis apparaissent soit par le côté droit, soit par le côté gauche de l'écran, de manière aléatoire.
    *   Ils apparaissent à des intervalles de temps réguliers.
*   **Mouvement :**
    *   Ils se déplacent horizontalement à travers l'écran vers le côté opposé à leur point d'apparition.
    *   Leur vitesse de progression horizontale est constante.
    *   Verticalement, ils effectuent des "bonds" périodiques : ils "sautent" vers le haut puis retombent en suivant une courbe, tout en continuant leur progression horizontale (similaire au mouvement du joueur dans le jeu "Flappy Bird").
*   **Interaction :**
    *   **Élimination :** Les ennemis sont éliminés par un seul tir du revolver du joueur.
    *   Un effet visuel et sonore est joué lors de l'élimination d'un ennemi.
    *   **Collision avec le joueur :** Si un ennemi entre en collision avec le revolver du joueur, la partie est perdue.

**4. Conditions de Victoire et Défaite**

*   **Victoire :** Il n'y a pas de condition de victoire finale. L'objectif est de survivre et de marquer le plus de points possible.
*   **Défaite :**
    *   Le joueur perd si le revolver touche le "sol" (limite inférieure de l'écran).
    *   Le joueur perd si le revolver entre en collision avec un ennemi ("Flappy Bird").

**5. Score et Progression**

*   **Calcul du Score :**
    *   Le score augmente chaque fois qu'un ennemi est éliminé.
    *   Chaque ennemi éliminé rapporte un nombre constant de points.
*   **Difficulté :**
    *   La difficulté du jeu reste constante tout au long de la partie (pas d'augmentation de la vitesse des ennemis, de leur fréquence d'apparition, etc.).

**6. Aspects Visuels**

*   **Style Graphique :** Cartoon.
*   **Revolver :** Design cartoon.
*   **Ennemis ("Flappy Birds") :** Apparence "classique" de Flappy Bird, style cartoon.
*   **Sol :** Un sol simple et visible en bas de l'écran (ex: type sol de Mario en 2D).
*   **Arrière-plan :** À définir (peut être simple pour commencer).
*   **Effets Visuels :**
    *   Effet lors du tir (ex: flash au bout du canon, traînée de balle).
    *   Effet lors de l'élimination d'un ennemi (ex: petite explosion, disparition en plumes).
    *   Effet lors de la collecte d'un chargeur.

**7. Aspects Sonores**

*   **Musique :** Optionnel, une musique de fond entraînante mais pas trop distrayante.
*   **Effets Sonores (SFX) :**
    *   Tir du revolver.
    *   "Clic à vide" (tir sans munitions).
    *   Collecte d'un chargeur.
    *   Élimination d'un ennemi.
    *   Son de début de partie.
    *   Son de défaite (game over).
    *   Impact du revolver sur le sol (si différent du son de défaite général).

**8. Interface Utilisateur (UI)**

*   **Affichage du Score :** En haut au centre de l'écran, visible en permanence pendant le jeu.
*   **Affichage des Munitions :** Pas d'affichage numérique. Le joueur doit compter ses balles.
*   **Écrans :**
    *   **Écran de Début :** Message simple (ex: "Flappy Gun", "Appuyez pour commencer").
    *   **Écran de Fin de Partie (Game Over) :** Affiche le score final du joueur et une option/bouton pour "Rejouer".

**9. Éléments Non Inclus (pour cette version initiale)**

*   Power-ups (autres que les chargeurs de munitions).
*   Différents types d'ennemis ou de revolvers.
*   Niveaux multiples ou augmentation progressive de la difficulté.
*   Rotation du revolver induite par le recul (torque) - mais paramètre à prévoir.
*   Sauvegarde du meilleur score (peut être ajouté plus tard).