# MSE-2023-template
Шаблонный проект для гитхаба на курсе Промышленная разработка ПО


## Git-Flow:
  - У нас есть 3 ветки: main, dev, test
  - Каждая feature в отдельной ветке, которая создаётся из dev
  - При старте работ над feature делаетс 2 PR: dev и test
  - Ревью осуществляется в ветке test, исправления заливатся в feature-ветку
  - При релизе, из dev делается релизная ветка в которую коммитится up версии и делается 2 PR: dev и master
  - После того, как PR будет влит в мастер на него ставится тег с версией

### Версионирование
Каждую доставку вашего ПО маркируйте следующей схемой: A.B.C, где A — это глобальные изменения, ломающие обратную совместимость; B — доставка новых функций (работоспособность прошлых версий, соответственно, сохраняется); C — мелкие правки, патчи и горячие фиксы

## Actions
build_and_release111.yml работает в полуавтоматическом режиме. Для прибавления версии тега необходимо зайти в .github/workflows/build_and_release.yml и изменить на 123 строке `tag_name: v1.0.x` последнюю цифру в шаге Create Release.
