# vsftpd

##폴더 만들기
```bash
$ sudo apt-get install vsftpd
$ mkdir /var/ftp
$ chown <내아이디>:<그룹> /var/ftp
```

## ftp 전용 사용자 계정 만들기

```bash
sudo useradd -d /var/ftp -s /bin/false <아이디> -G <그룹>
sudo echo "암호" | passwd <아이디>

```

참고
```bash
# 그룹 만들기
$ sudo addgroup ftpuser

# 사용자 계정 그룹 수정
$ sudo usermod -a -G <아이디> <그룹명>
```
```bash
#일반 계정 만들기
$ useradd <아이디> -m -s /bin/bash -G <그룹>
	# -m : 홈 디렉토리 생성
	# -s : 쉘 지정
	# -G : 그룹 지정
```

```bash
# 홈디렉토리 변경
$ usermod -d <폴더> <아이디>
```


# /etc/vsftpd.conf

```bash
#익명 로그인 막기
anonymous_enable=NO		

# 개인 설정하려면 필요함
user_config_dir=/etc/vsftpd/vsftpd_user_conf 

# 쉘 로그인 막으려고 설정을 /bin/false 로 바꾸면 ftp로그인이 안되므로 바꿔준다.
#pam_service_name=vsftpd
pam_service_name=ftp	

# 나머지는 아마도 기본값
local_enable=YES
write_enable=YES
local_umask=022
dirmessage_enable=YES
use_localtime=YES
xferlog_enable=YES
connect_from_port_20=YES
chroot_local_user=YES
allow_writeable_chroot=YES
chroot_local_user=YES
chroot_list_enable=YES
chroot_list_file=/etc/vsftpd.chroot_list
secure_chroot_dir=/var/run/vsftpd/empty
pam_service_name=ftp
rsa_cert_file=/etc/ssl/private/vsftpd.pem
```

# /etc/vsftpd/vsftpd_user_conf

```bash
# 업로드 전용
ascii_download_enable=NO  
# 루트 디렉토리
local_root = /var/ftp
# 사용가능한 명령어
cmds_allowed = LIST,CWD,STOR,QUIT,PASS,PASV,PWD,ACCT,USER,STAT,NOOP,PORT,NLIST,SITE,ABOR,SYST,TYPE,STRU
```


# 재시작

```bash
sudo service vsftpd restart
```