## 2022-01-OSSP1-ElegantSiblings-3
- 주제: 실시간 PVP대전과 다양한 모드를 지원하는 '2048 Master'<br>
<br>
<br>


## 팀원 역할
|학과|이름|역할|담당 기술|
|-------|-----|-----|-----|
|컴퓨터공학과|문정훈|팀장|게임 클라이언트(Unity), 실시간 멀티플레이 서버(C# TCP/IP Socket)|
|경영정보학과|김혜연|팀원|게임 디자인(Adobe Illustrator), Database(RDS, MySQL)|
|컴퓨터공학과|신준오|팀원|Database(RDS, MySQL)|
|컴퓨터공학과|장유진|팀원|게임 디자인(Adobe Illustrator)|
<br>
<br>


## 2022 1학기 공개SW프로젝트 A+
<img src="https://github.com/Dice15/2022-01-OSSP1-2048-Master/assets/102275981/24a3a20f-54fd-46a5-857d-e471d5bcc267" width="100%">
<br>
<br>
<br>


## 실시간 PVP대전과 다양한 모드를 지원하는 '2048 Master'

* **지원하는 OS**
  * Android Mobile
  * Windows
<br>
<br>


* **개발 환경**
  * Game Client: Unity (2021.3.1f1 LTS)
  * Game Server: C# (.NET Framework 3.5)
  * Database: Amazon RDS MySQL  
<br>
<br>


* **개발 내용**
  * 싱글플레이
    * 클래식 모드
    * 도전 모드
    * 연습 모드 
  <br>

  * 멀티플레이
    * PVP: 일반 대전
    * PVP: 아이템 대전 (프로젝트 간소화로 취소)  
  <br>
  
  * 실시간 멀티플레이 서버
    * 매칭 시스템
    * 게임 룸
    * 실시간 통신
  <br>
  
  * 계정 시스템
    * 플레이어 정보 (닉네임, 게임 전적 등)
    * 게임 데이터 보존 (다른 기기에서 이어서 플레이 가능) 
  <br>
  
  * 테마 시스템
<br>
<br>


* **Demo 플레이 화면**
  * 멀티플레이: 시연용 버전이므로 2048 -> 128로 간소화
  <br><img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/GIF_PVP.gif" width="622" height="350"/><br>
  
  * 싱글플레이: Challenge 모드
  <br><img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/GIF_Challenge.gif" width="622" height="350"/><br>
  
  * 테마
  <br><img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/GIF_Theme.gif" width="622" height="350"/><br> 
<br>
<br>


* **Realtime Multiplayer Server (실시간 PVP대전 서버)**
  * Photon Engine, Amazon GameLift를 사용하거나 Server API를 쓰려고 했으나<br>
    교수님의 의견을 반영하여 직접 자체 서버를 구현하게 되었다
  
  <br><img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/Server_Logic1.png" width="622" height="350"/>
  <img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/Server_Logic2.png" width="622" height="350"/>
  <img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/Server_Logic3.png" width="622" height="350"/>
  <img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/Server_Logic4.png" width="622" height="350"/>
<br>
<br>


* **User Account System (유저 계정 시스템)**
  * DB(Mysql)로 유저 계정관리
  
  <br><img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/User_Account1.png" width="622" height="350"/>
  <img src="https://github.com/CSID-DGU/2022-01-OSSP1-ElegantSiblings-3/blob/main/Presentation/User_Account2.png" width="622" height="350"/>
<br>
<br>


