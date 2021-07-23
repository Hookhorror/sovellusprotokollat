# TIES323 - Sovellusprotokollat

## Tehdyt tehtävät

### Mail Protocols

#### Tehtävä 1 - mail_protocols_1
SMTP serveriin saa yhteyden telnetillä portista 25000, jonka jälkeen keskustelu hoituu komennoilla HELO, MAIL TO, RCPT TO, DATA ja QUIT. Komennot on annettava isolla kirjoitettuna.
#### Tehtävä 2 - mail_protocols_2
Tehtävää varten asensin Dovecot sähköposti palvelimen koneelle, jolta ohjelma hakee tätä tehtävää varten tehdyllä käyttäjällä sähköpostilistauksen käyttäen POP3 protokollaa.

### File Transfer Protocols
#### Tehtävä 1 - file_transfer_protocols_1
Ottaa yhteyden oletuskäyttäjänä ennalta määrättyyn ftp palvelimeen (vsftp), jonka jälkeen käyttäjä voi itse antaa komentoja LIST, RETR EPSV tai QUIT.
#### Tehtävä 2 - file_transfer_protocols_2
TFTP asiakas joka osaa tehdä RRQ pyynnön ja vastaanottaa tiedoston ennaltamäärätystä palvelimesta. Jos haluaa testata tehtävä3:n palvelimen kanssa niin porttinumero on muutettava 6969:ksi.
#### Tehtävä 3 - file_transfer_protocols_3
TFTP palvelin joka osaa lähettää tiedoston asiakkaalle. Käyttää porttia 6969.
#### Tehtävä 4
##### file_transfer_protocols_4_client
RRQ tai WRQ pyyntö on muutettava koodissa, samoin kuin ladattavan tai lähetettävän tiedoston nimi.
##### file_transfer_protocols_4_server
Osaa automaattisesti tulkita onko pyyntö WRQ vai RRQ ja toimii sen mukaan.

### Own Choice of Protocols
#### Tehtävä 1 - own_choice_irc_client
Kirjautuu automaattisesti ennaltamäärätylle irc palvelimelle, lähettää whois komennon omalle käyttäjänimelleen ja lähtee pois.

### Netkit
#### Web server - netkit/webservers
#### CGI - netkit/cgi
#### DNS - netkit/dns
#### Walkthrough - netkit/walkthrough
#### Load balancing Web switch - netkit/load_balancing
#### Load balancing DNS - netkit/load_balancing_dns
#### E-Mail - netkit/email

### Setup a real server
#### real_server
Serverinä toimi RaspberryPi johon asennettiin tuore Raspberry Pi OS Lite. Reititin oli vanha Buffalon nurkissa lojunut reititin, josta poistettiin DHCP käytöstä. Asiakkaana toimi oma kannettava tietokoneeni. Kuvia laitteista löytyy osoitteesta http://users.jyu.fi/~joankivi/ties323/ .

Laitteet konfiguroitiin Kathara-tehtävistä tutuilla tavoilla. Käytetyt palvelut ja sovellukset olivat bind9 DNS:lle, exim4 sähköpostin välitykselle, dovecot sähköpostille (smtp, pop, imap) ja Apache2 web-palvelimena.
