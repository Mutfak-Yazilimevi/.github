#!/usr/bin/env python3
"""sec-* skill'lerini güvenlik alt-plugin'lerine sınıflandırır → sec-routing.tsv
6 alt-plugin: offensive, detection, forensics, intel, grc, defense (default).
Sıra önemli: detection/forensics/intel önce, sonra offensive, grc; kalan defense."""
import sys, glob, os
SRC=sys.argv[1] if len(sys.argv)>1 else 'project-template/.claude'
def bucket(s):  # s = 'sec-...'
    # 1) DETECTION (mavi takım tespit/avlama) — 'detecting-*-attacks' offensive'e kaçmasın
    if s.startswith('sec-detecting') or s.startswith('sec-hunting') or s.startswith('sec-triaging'):
        return 'detection'
    det=('-siem','siem-','soc-','-soc','alert-','false-positive','detection-rule','user-behavior','ueba',
         'beaconing','with-splunk','with-zeek','with-sigma','-qradar','sentinel','elastic-siem',
         'correlating-','analyzing-security-logs','analyzing-windows-event-logs','analyzing-azure-activity',
         'analyzing-office365-audit','analyzing-cloudtrail','analyzing-api-gateway-access','analyzing-dns-logs',
         'analyzing-linux-audit','analyzing-kubernetes-audit','analyzing-network-flow','analyzing-web-server-logs',
         'analyzing-powershell-script-block','monitoring-scada','analyzing-network-traffic-for-incidents')
    if any(k in s for k in det): return 'detection'
    # 2) FORENSICS / DFIR / malware RE
    for_=('forensic','reverse-engineering','-malware','deobfuscating','extracting-','recovering-','carving',
          'timeline','investigating-','volatility','autopsy','rekall','plaso','prefetch','-mft','shellbag',
          'amcache','lnk-file','jump-list','pst-for-email','sqlite-database','browser-history','slack-space',
          'analyzing-windows-registry','analyzing-windows-prefetch','analyzing-windows-amcache','analyzing-usb',
          'analyzing-disk-image','analyzing-memory','analyzing-bootkit','analyzing-linux-kernel-rootkits',
          'analyzing-linux-system-artifacts','analyzing-docker-container-forensics','analyzing-uefi',
          'config-from','collecting-volatile','acquiring-disk-image','extracting-credentials','extracting-iocs',
          'extracting-config','extracting-browser','extracting-memory','extracting-windows','steganography',
          'firmware-extraction','firmware-malware','file-carving','analyzing-android-malware','analyzing-ios-app',
          'analyzing-golang-malware','analyzing-linux-elf','analyzing-macro-malware','analyzing-malicious-pdf',
          'analyzing-malicious-url','analyzing-packed-malware','analyzing-pdf-malware','analyzing-cobalt-strike-beacon-config',
          'analyzing-cobaltstrike','analyzing-powershell-empire','analyzing-network-traffic-of-malware',
          'analyzing-network-covert','analyzing-ransomware-encryption','analyzing-ransomware-network',
          'analyzing-ransomware-payment','analyzing-supply-chain-malware','analyzing-malware')
    if any(k in s for k in for_): return 'forensics'
    # 3) THREAT INTEL (CTI)
    intel=('threat-intelligence','threat-intel','threat-actor','threat-feed','threat-landscape','threat-campaign',
           'darkweb','dark-web','osint','open-source-intelligence','-stix','stix-','-taxii','taxii-','-misp','misp-',
           'opencti','intelligence-lifecycle','intelligence-platform','campaign-attribution','diamond-model',
           'profiling-threat','tracking-threat','correlating-threat','generating-threat-intelligence',
           'managing-intelligence','indicator-lifecycle','attack-pattern-library','paste-site','brand-monitoring',
           'ip-reputation','ioc-enrichment','ioc-defanging','collecting-indicators','collecting-threat',
           'collecting-open-source','processing-stix','automating-ioc','adversary-infrastructure-tracking',
           'building-threat','monitoring-darkweb','mapping-mitre-attack','analyzing-threat','analyzing-indicators',
           'analyzing-apt-group','analyzing-campaign','analyzing-threat-actor','analyzing-threat-intelligence',
           'analyzing-threat-landscape','analyzing-typosquatting','analyzing-certificate-transparency',
           'analyzing-tls-certificate','building-attack-pattern','building-adversary')
    if any(k in s for k in intel): return 'intel'
    # 4) OFFENSIVE (red team / pentest / exploit)
    off=('exploiting-','penetration-test','red-team','-attack-simulation','bypassing-','cracking','kerberoast',
         'pass-the-hash','pass-the-ticket','golden-ticket','silver-ticket','dcsync','social-engineering',
         'spearphishing-simulation','phishing-simulation','spearphishing-campaign','-c2','c2-infrastructure',
         'conducting-','executing-red-team','executing-active-directory-attack','executing-phishing-simulation',
         'intercepting-mobile','arp-spoofing','ssl-stripping','packet-injection','man-in-the-middle','vlan-hopping',
         'testing-for-','testing-api','testing-jwt','testing-mobile-api','testing-oauth2','testing-websocket','testing-cors',
         'second-order-sql','blind-ssrf','http-request-smuggling','http-parameter-pollution','csrf-attack','clickjacking',
         'jwt-none','graphql-introspection','graphql-depth','ssrf-vulnerability-exploitation','web-cache-deception',
         'web-cache-poisoning','web-application-firewall-bypass','content-security-policy-bypass','certificate-pinning-bypass',
         'binary-exploitation','fuzzing','hash-cracking','wifi-password','performing-active-directory-penetration',
         'performing-active-directory-forest-trust-attack','performing-privilege-escalation-on','performing-privilege-escalation-assessment',
         'performing-aws-privilege-escalation','performing-lateral-movement-with','performing-credential-access',
         'performing-kerberoasting','performing-initial-access','performing-domain-persistence','performing-red-team',
         'performing-purple-team','performing-threat-emulation','performing-physical-intrusion','heap-spray','process-hollowing-technique',
         'performing-arp','performing-bandwidth','performing-bluetooth-security-assessment','performing-thick-client',
         'performing-wireless-network-penetration','performing-wireless-security','performing-iot-security-assessment',
         'performing-external-network-penetration','performing-internal-network','performing-mobile-app','exploiting-active-directory',
         'building-c2','building-red-team','building-adversary-infrastructure')
    if any(k in s for k in off): return 'offensive'
    # 5) GRC (audit / vuln-mgmt / compliance / IR / IAM-governance)
    grc=('auditing-','-audit','scanning-','-scanning','vulnerability','assessment','compliance','iso-27001','iso27001',
         'pci-dss','nist-csf','soc2','gdpr','nerc-cip','iec-62443','prioritizing-vuln','cvss','epss','-kev','ssvc','-sla',
         'incident-response','incident-commander','containing-','eradicating-','ransomware-response','recovering-from-ransomware',
         'post-incident','tabletop','lessons-learned','-playbook','playbook-','privacy-impact','risk-management','remediation',
         'patch-management','recertification','access-review','entitlement','privileged-account-discovery','privileged-account-access',
         'identity-governance','access-recertification','service-account-audit','cryptographic-audit','security-headers-audit',
         'cis-benchmark','docker-bench','kube-bench','agentless-vulnerability','authenticated-scan','authenticated-vulnerability',
         'asset-criticality','asset-discovery','attack-surface-management','attack-path-analysis','exception-tracking',
         'aging-and-sla','dashboard-with-defectdojo','vulnerability-dashboard','vulnerability-triage','epss-score')
    if any(k in s for k in grc): return 'grc'
    # 6) DEFENSE (kalan: implementing/configuring/securing/hardening/deploying kontroller, zero-trust, kimlik)
    return 'defense'

names=sorted(d.split('/skills/')[1].rstrip('/') for d in glob.glob(os.path.join(SRC,'skills','sec-*'))+glob.glob(os.path.join(SRC,'skills','sec-*/')))
names=sorted(set(n for n in names if n.startswith('sec-')))
out=os.path.join(SRC,'skills','sec-routing.tsv')
with open(out,'w',encoding='utf-8') as f:
    for n in names:
        f.write(f"{n}\tmutfak-security-{bucket(n)}\n")
from collections import Counter
c=Counter(bucket(n) for n in names)
print("yazıldı:",out,"|",len(names),"skill")
for b,k in c.most_common(): print(f"  mutfak-security-{b:<10}{k}")
