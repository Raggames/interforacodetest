# interforacodetest


<b> Le projet : </b>

Au lancement, la position (ainsi que d'autres donnnées que je n'ai pas eu le temps d'afficher) de trois objets différents est chargée depuis la base de données. Ils sont instantiés par un manager et il est possible de les déplacer aléatoirement via un bouton GUI qui mettra aussi à jour les données du backend.
La position des objets est donc persistente d'une session à l'autre (raffraichir la page pour tester).

<b> Résumé du déroulement : </b>

Création d'un game Manager qui permet de gérer le chargement et l'input utilisateur (via OnGUI) pour une simple fonction de test.

Implémentation d'un pattern repository modulaire issu de projets personnels et adaptation de la communication HTTP au build WebGL (les HTTP Request C# n'étant pas gérées par WebGL build).
Utilisation/Adaptation de scripts CRUD d'un backend personnel existant en PHP, création d'une nouvelle base sur mon serveur personnel hostinger. C'était la façon la plus rapide que j'avais pour déployer 
des fonctionnalités backend fonctionnelles, mais à choisir avec du temps, je l'aurais fait en .NET Core Entity/Code First.

Mise en place de la gestion des objets coté jeu.
Tests en éditeur.

Après environ 1h20 , le projet est fonctionnel en éditeur, mais je rencontre un problème de communication avec le backend en build.
Je suis déjà hors timing, mais j'ai envie de comprendre ce qui ne fonctionne pas.

Après 40 minutes de recherches et grâce au forum unity, la solution trouvée. Il s'agit de la configuration CORS du serveur qui n'est pas bonne et empêchait la communication depuis UNITY.
Après 5 minutes, les scripts PHP sont mis à jour, et tout fonctionne en build.

Ce qui représente environ 2h de temps au total.

<b> Remarques <b/>

Je ne suis pas certain d'avoir cerné la consigne, mais je suis plutôt satisfait d'avoir quelque chose de fonctionnel en moins de deux heures !
