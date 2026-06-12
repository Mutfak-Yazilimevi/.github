# Agent İndeksi

> `project-template/.claude/agents/` altındaki tüm sub-agent'ların **kategori bazlı** listesi.
> Her satır: agent adı, model ve amacı (`description`). Çağrı adı agent'ın frontmatter `name`
> alanından gelir — dosya konumu/kategori çağrı adını **etkilemez** (saf kataloglama).
> **Toplam:** 64 agent · **Son tarama:** 2026-06-12

> **Model seçimi:** en az token + ortalama-üstü performans ilkesi — mekanik/yapılandırılmış işler `haiku`, ciddi muhakeme/mimari/uzmanlık `sonnet`, en karmaşık orkestrasyon `opus`.

> **Kategoriler:** [Orkestrasyon & Spec](#orkestrasyon--spec-workflow) ·
> [Backend & Mimari](#backend--mimari) · [Dil & Platform](#dil--platform-uzmanları) ·
> [AI/ML & Veri](#aiml--veri) · [DevOps / SRE / Cloud](#devops--sre--cloud) ·
> [Kalite](#kalite--test--review--debug--performans) · [Dokümantasyon](#dokümantasyon) ·
> [.NET](#net) · [Frontend & UI/UX](#frontend--uiux) · [Güvenlik](#güvenlik) ·
> [Ürün Yönetimi](#ürün-yönetimi) · [Pazarlama / Blog](#pazarlama--blog)

---

## Orkestrasyon & Spec Workflow

> Plugin: `mutfak-spec-workflow` · Çok ajanlı koordinasyon, spec-driven iş akışı, takım kurulumu. **(12 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `agent-organizer` | haiku | Karmaşık, çok ajanlı görevler için ana orkestratör görevi gören üst düzey gelişmiş bir yapay zeka ajanı. Proje gereksinimlerini analiz eder, uzmanlaşmış yapay zeka ajanlarından oluşan bir ekip tanımlar ve proje hedeflerine ulaşmak için onların iş birliğine dayalı iş akışını yönetir. Kapsamlı proje analizi, stratejik ajan ekibi oluşturma ve dinamik iş akışı yönetimi için PROAKTİF olarak kullanın. |
| `tech-lead-orchestrator` | opus | Karmaşık yazılım projelerini analiz eden ve stratejik öneriler sunan kıdemli teknik lider. Çok adımlı her geliştirme görevi, özellik uygulaması veya mimari karar için MUTLAKA KULLANILMALIDIR. Optimum ajan koordinasyonu için yapılandırılmış bulgular ve görev dökümleri döndürür. |
| `team-configurator` | haiku | Mevcut proje için yapay zekâ geliştirme takımını kurmak veya yenilemek üzere MUTLAKA KULLANILMALIDIR. Yeni depolarda, büyük teknoloji yığını değişikliklerinden sonra veya kullanıcı yapay zekâ takımını yapılandırmayı istediğinde PROAKTİF olarak kullanın. Yığını tespit eder, en uygun uzman alt ajanları seçer ve CLAUDE.md dosyasını bir "AI Team Configuration" bölümüyle yazar/günceller. |
| `project-analyst` | sonnet | Herhangi bir yeni veya tanıdık olmayan kod tabanını analiz etmek için MUTLAKA KULLANILMALIDIR. Uzmanların doğru şekilde yönlendirilebilmesi için framework'leri, teknoloji yığınlarını ve mimariyi tespit etmek üzere PROAKTİF olarak kullanın. |
| `spec-analyst` | sonnet | Gereksinim analisti ve proje kapsam belirleme uzmanı. Kapsamlı gereksinimleri ortaya çıkarma, kabul kriterli kullanıcı hikayeleri oluşturma ve proje özetleri üretme konularında uzmanlaşmıştır. İhtiyaçları netleştirmek için stakeholder'larla çalışır ve fonksiyonel/fonksiyonel olmayan gereksinimleri yapılandırılmış formatlarda belgeler. |
| `spec-architect` | sonnet | Teknik tasarım ve mimaride uzmanlaşmış sistem mimarı. Kapsamlı sistem tasarımları, teknoloji yığını önerileri, API spec'leri ve veri modelleri oluşturur. İş gereksinimleriyle uyumu korurken ölçeklenebilirlik, güvenlik ve sürdürülebilirlik sağlar. |
| `spec-planner` | sonnet | Mimari tasarımları eyleme dönük görevlere ayıran uygulama planlama uzmanı. Ayrıntılı görev listeleri oluşturur, karmaşıklığı tahmin eder, uygulama sırasını tanımlar ve kapsamlı test stratejileri planlar. Tasarım ile geliştirme arasındaki boşluğu kapatır. |
| `spec-developer` | sonnet | Spesifikasyonlara dayalı olarak özellikleri hayata geçiren uzman geliştirici. Mimari desenleri ve en iyi uygulamaları izleyerek temiz, bakımı kolay kod yazar. Birim testleri oluşturur, hata durumlarını ele alır ve kodun performans gereksinimlerini karşılamasını sağlar. |
| `spec-tester` | sonnet | Test paketleri oluşturan ve yürüten kapsamlı bir test uzmanı. Birim testleri, entegrasyon testleri ve E2E testleri yazar. Güvenlik testi, performans testi gerçekleştirir ve kod kapsamının standartları karşılamasını sağlar. Kaliteyi korumak için spec-developer ile yakın çalışır. |
| `spec-reviewer` | sonnet | Kod kalitesi, en iyi uygulamalar ve güvenlik konusunda uzmanlaşmış kıdemli kod inceleyicisi. Kodu sürdürülebilirlik, performans optimizasyonları ve olası güvenlik açıkları açısından inceler. Eyleme dönük geri bildirim sağlar ve kodu doğrudan yeniden düzenleyebilir (refactor). Tutarlı kaliteyi sağlamak için tüm uzman ajanlarla çalışır. |
| `spec-validator` | sonnet | Gereksinim uyumluluğunu ve üretime hazırlığı güvence altına alan son kalite doğrulama uzmanı. Tüm gereksinimlerin karşılandığını, mimarinin düzgün biçimde uygulandığını, testlerin geçtiğini ve kalite standartlarına ulaşıldığını doğrular. Kapsamlı doğrulama raporları ve kalite skorları üretir. |
| `spec-orchestrator` | sonnet | Proje organizasyonu, kalite kapısı (quality gate) yönetimi ve ilerleme takibine odaklanan iş akışı koordinasyon uzmanı. Doğrudan ajan yönetimi olmaksızın stratejik planlama ve koordinasyon yetenekleri sunar. |

---

## Backend & Mimari

> Plugin: `mutfak-dev` · Sistem mimarisi, API tasarımı, full-stack, legacy modernizasyon. **(6 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `backend-architect` | sonnet | Sağlam, ölçeklenebilir ve sürdürülebilir backend sistemleri tasarlamak için danışman bir mimar olarak hareket eder. Bir çözüm önermeden önce ilk olarak Context Manager'a danışarak ve ardından açıklayıcı sorular sorarak gereksinimleri toplar. |
| `senior-backend-architect` | sonnet | Google'da 10+ yıl deneyime sahip, 10M+ kullanıcılı birden fazla ürünü yöneten kıdemli backend mühendisi ve sistem mimarı. Go ve TypeScript uzmanı; dağıtık sistemler, yüksek performanslı API'ler ve üretim seviyesinde altyapı konularında uzmanlaşmıştır. Hem teknik uygulamada hem de sistem tasarımında ustadır; sıfır kesintili dağıtımlar ve asgari düzeyde üretim olayı geçmişine sahiptir. |
| `full-stack-developer` | sonnet | Web uygulamalarının kullanıcı arayüzünden sunucu tarafı mantığına ve veritabanı yönetimine kadar her yönünü tasarlamada, geliştirmede ve sürdürmede yetkin, çok yönlü bir yapay zeka Full Stack Geliştirici. Tüm teknoloji yığını boyunca sorunsuz entegrasyon ve işlevsellik sağlayarak uçtan uca uygulama geliştirme için PROAKTİF olarak kullanın. |
| `graphql-architect` | sonnet | Yüksek performanslı, ölçeklenebilir ve güvenli GraphQL API'lerini tasarlamak, uygulamak ve optimize etmek için son derece uzmanlaşmış bir yapay zeka ajanı. Şema mimarisinde, resolver optimizasyonunda, federe edilmiş servislerde ve subscription'larla gerçek zamanlı veride başarılıdır. Bu ajanı sıfırdan başlatılan GraphQL projeleri, performans denetimi veya mevcut GraphQL API'lerinin yeniden düzenlenmesi için kullanın. |
| `architect-reviewer` | haiku | Mimari tutarlılık, desenlere uyum ve sürdürülebilirlik açısından kodu proaktif olarak inceler. Sistem bütünlüğünü sağlamak için herhangi bir yapısal değişiklikten, yeni servis tanıtımından veya API değişikliğinden sonra kullanın. |
| `legacy-modernizer` | sonnet | Eski (legacy) sistemlerin kademeli modernizasyonunu planlamak ve yürütmek için bir uzman ajan. Yaşlanan kod tabanlarını yeniden düzenler, modası geçmiş framework'leri taşır ve monolitleri güvenli bir şekilde parçalara ayırır. Teknik borcu azaltmak, sürdürülebilirliği artırmak ve operasyonları aksatmadan teknoloji yığınlarını yükseltmek için bunu kullanın. |

---

## Dil & Platform Uzmanları

> Plugin: `mutfak-dev` · Dile/platforma özgü idiomatik geliştirme uzmanları. **(5 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `python-pro` | sonnet | Temiz, performanslı ve deyimsel (idiomatic) kod yazmada uzmanlaşmış uzman bir Python geliştiricisi. Dekoratörler, generator'lar ve async/await dahil ileri düzey Python özelliklerinden yararlanır. Performansı optimize etmeye, yerleşik tasarım desenlerini uygulamaya ve kapsamlı test kapsamı sağlamaya odaklanır. Python refactoring'i, optimizasyonu veya karmaşık özelliklerin uygulanması için PROAKTİF olarak kullanın. |
| `golang-pro` | sonnet | Sağlam, eşzamanlı ve yüksek performanslı Go uygulamaları mimarisini kuran, yazan ve yeniden düzenleyen bir Go uzmanı. Deyimsel koda, uzun vadeli sürdürülebilirliğe ve operasyonel mükemmelliğe odaklanarak tasarım tercihleri için ayrıntılı açıklamalar sunar. Mimari tasarım, derinlemesine kod incelemeleri, performans ayarı ve karmaşık eşzamanlılık zorlukları için PROAKTİF olarak kullanın. |
| `typescript-pro` | sonnet | Node.js ve tarayıcı ortamları için ölçeklenebilir, tip güvenli ve sürdürülebilir uygulamaları tasarlayan, yazan ve yeniden düzenleyen bir TypeScript uzmanı. Mimari kararları için ayrıntılı açıklamalar sunar; deyimsel koda, sağlam testlere ve kod tabanının uzun vadeli sağlığına odaklanır. Mimari tasarım, karmaşık tip düzeyinde programlama, performans ayarı ve büyük kod tabanlarının yeniden düzenlenmesi için PROAKTİF olarak kullanın. |
| `electron-pro` | sonnet | Electron ve TypeScript kullanarak platformlar arası masaüstü uygulamaları geliştirmede uzman. Web teknolojilerinin masaüstü ortamındaki tüm potansiyelinden yararlanarak güvenli, performanslı ve sürdürülebilir uygulamalar oluşturmada uzmanlaşmıştır. Sağlam süreçler arası iletişime, yerel (native) sistem entegrasyonuna ve kusursuz bir kullanıcı deneyimine odaklanır. Yeni Electron uygulamaları geliştirme, mevcut uygulamaları refactor etme veya karmaşık masaüstüne özgü özellikleri uygulama için PROAKTİF olarak kullanın. |
| `mobile-developer` | sonnet | React Native ve Flutter kullanarak gelişmiş, çapraz platform mobil uygulamaların geliştirilmesini tasarlar ve yönetir. Bu rol; sağlam yerel (native) entegrasyonlar, ölçeklenebilir mimari ve kusursuz kullanıcı deneyimleri sağlamak için mobil stratejide proaktif liderlik gerektirir. Temel sorumluluklar arasında çevrimdışı veri senkronizasyonunu yönetmek, kapsamlı push bildirim sistemleri uygulamak ve uygulama mağazası dağıtımlarının karmaşıklıklarında yol almak yer alır. |

---

## AI/ML & Veri

> Plugin: `mutfak-dev` (`prompt-engineer` → `mutfak-core`) · LLM/RAG, ML yaşam döngüsü, veri pipeline & veritabanı. **(7 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `ai-engineer` | sonnet | LLM destekli uygulamalar, RAG sistemleri ve karmaşık prompt pipeline'ları tasarlamak, oluşturmak ve optimize etmek için son derece uzmanlaşmış bir yapay zeka ajanı. Bu ajan vektör araması uygular, agentic iş akışlarını orkestre eder ve çeşitli yapay zeka API'leriyle entegrasyon sağlar. LLM özelliklerini, chatbot'ları veya herhangi bir yapay zeka odaklı uygulamayı geliştirmek ve iyileştirmek için PROAKTİF olarak kullanın. |
| `ml-engineer` | sonnet | Üretimdeki makine öğrenmesi modellerinin uçtan uca yaşam döngüsünü tasarlar, inşa eder ve yönetir. Ölçeklenebilir, güvenilir ve otomatikleştirilmiş ML sistemleri oluşturmada uzmanlaşmıştır. ML modellerinin dağıtımı, izlenmesi ve bakımını içeren görevler için PROAKTİF olarak kullanın. |
| `data-engineer` | sonnet | ETL/ELT pipeline'ları, veri ambarları ve gerçek zamanlı akış (streaming) mimarileri dahil olmak üzere ölçeklenebilir ve sürdürülebilir, veri yoğun uygulamalar tasarlar, kurar ve optimize eder. Bu ajan Spark, Airflow ve Kafka konusunda uzmandır ve veri yönetişimi ile maliyet optimizasyonu ilkelerini proaktif olarak uygular. Yeni veri çözümleri tasarlamak, mevcut veri altyapısını optimize etmek veya veri pipeline sorunlarını gidermek için kullan. |
| `data-scientist` | sonnet | İleri düzey SQL, BigQuery optimizasyonu ve eyleme dönük veri öngörülerinde uzmanlaşmış bir veri bilimci uzmanı. Veri keşfi ve analizinde işbirlikçi bir ortak olacak şekilde tasarlanmıştır. |
| `database-optimizer` | sonnet | Veritabanı performansını bütünsel olarak analiz edip optimize eden bir yapay zeka asistanı uzmanı. SQL sorguları, indeksleme, şema tasarımı ve altyapıyla ilgili darboğazları belirler ve giderir. Performans ince ayarı, şema iyileştirmesi ve göç planlaması için proaktif olarak kullan. |
| `postgresql-pglite-pro` | sonnet | PostgreSQL ve Pglite konusunda uzman; sağlam veritabanı mimarisi, performans iyileştirme ve tarayıcı içi veritabanı çözümlerinin uygulanması üzerine uzmanlaşmıştır. Verimli veri modelleri tasarlamada, sorguları hız ve güvenilirlik açısından optimize etmede ve yenilikçi web uygulamaları için Pglite'tan yararlanmada üstündür. Veritabanı tasarımı, sorgu optimizasyonu ve istemci tarafı veritabanı işlevlerinin uygulanması için PROAKTİF olarak kullanın. |
| `prompt-engineer` | sonnet | Karmaşık LLM etkileşimlerini tasarlayan ve optimize eden usta düzeyinde bir prompt mühendisi. Gelişmiş yapay zeka sistemleri tasarlamak, model performansını sınırlarına kadar zorlamak ve sağlam, güvenli ve güvenilir ajan tabanlı (agentic) iş akışları oluşturmak için kullanın. Çok çeşitli ileri düzey prompt teknikleri, modele özgü incelikler ve etik yapay zeka tasarımı konusunda uzmandır. *(plugin: `mutfak-core`)* |

---

## DevOps / SRE / Cloud

> Plugin: `mutfak-dev` · CI/CD, bulut altyapısı, olay müdahalesi, geliştirici deneyimi. **(5 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `cloud-architect` | sonnet | Ölçeklenebilir, güvenli ve maliyet açısından verimli AWS, Azure ve GCP altyapıları tasarlayan kıdemli bir bulut mimarı yapay zekası. Infrastructure as Code (IaC) için Terraform konusunda uzmanlaşmıştır, maliyet optimizasyonu için FinOps en iyi uygulamalarını hayata geçirir ve çok bulutlu (multi-cloud) ile serverless çözümler mimarisini kurar. Altyapı planlaması, maliyet azaltma analizi veya bulut göç stratejileri için PROAKTİF olarak devreye girer. |
| `deployment-engineer` | sonnet | Sağlam CI/CD pipeline'ları, konteyner orkestrasyonu ve bulut altyapısı otomasyonu tasarlar ve uygular. DevOps ve GitOps en iyi uygulamalarını kullanarak ölçeklenebilir, üretim sınıfı deployment iş akışlarını proaktif olarak mimari kurar ve güvenli hale getirir. |
| `devops-incident-responder` | sonnet | Olay müdahalesini yönetmek, derinlemesine kök neden analizi yapmak ve üretim sistemleri için sağlam düzeltmeler uygulamak üzere uzmanlaşmış bir ajan. Bu ajan, sistem kesintilerini ve performans düşüşlerini proaktif olarak tespit edip çözmek için izleme ve gözlemlenebilirlik araçlarından yararlanma konusunda uzmandır. |
| `incident-responder` | sonnet | Google SRE ve diğer sektör en iyi uygulamalarına dayanarak, kritik üretim olaylarına aciliyet, hassasiyet ve net iletişimle müdahaleyi yöneten, savaş meydanında sınanmış bir Olay Komutanı (Incident Commander) personası. Üretim sorunları meydana geldiğinde HEMEN kullanın. |
| `dx-optimizer` | sonnet | Geliştirici Deneyimi (DX) uzmanı. Amacım, özellikle yeni projeler başlatırken, ekip geri bildirimlerine yanıt verirken veya geliştirme sürecinde sürtünme tespit edildiğinde araçları, kurulumu ve iş akışlarını proaktif olarak iyileştirmektir. |

---

## Kalite — Test / Review / Debug / Performans

> Plugin: `mutfak-dev` · Hata ayıklama, KG, test otomasyonu, kod incelemesi, refactor, performans. **(6 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `debugger` | sonnet | Hatalar, test başarısızlıkları ve beklenmeyen davranışlar için hata ayıklama uzmanı. Herhangi bir sorunla karşılaşıldığında proaktif olarak kullan. |
| `qa-expert` | sonnet | Yazılım ürünlerinin en yüksek kalite, güvenilirlik ve kullanıcı memnuniyeti standartlarını karşılamasını sağlamak üzere kapsamlı KG süreçlerini tasarlayan, uygulayan ve yöneten gelişmiş bir yapay zeka Kalite Güvencesi (KG) Uzmanı. Test stratejileri geliştirmek, ayrıntılı test planlarını yürütmek ve geliştirme ekiplerine veri odaklı geri bildirim sağlamak için PROAKTİF olarak kullanın. |
| `test-automator` | haiku | Kapsamlı bir otomatik test stratejisini tasarlamaktan, uygulamaktan ve sürdürmekten sorumlu bir Test Otomasyonu Uzmanı. Bu rol, sağlam test paketleri oluşturmaya, test için CI/CD hatlarını kurmaya ve yönetmeye ve yazılım geliştirme yaşam döngüsü boyunca yüksek kalite ve güvenilirlik standartları sağlamaya odaklanır. Test kapsamını iyileştirmek, test otomasyonunu sıfırdan kurmak veya test süreçlerini optimize etmek için PROAKTİF olarak kullanın. |
| `code-reviewer-pro` | haiku | Kapsamlı kod incelemeleri yürüten, yapay zeka destekli kıdemli bir mühendislik lideri. Kodu kalite, güvenlik, sürdürülebilirlik ve en iyi uygulamalara uygunluk açısından analiz eder; net, eyleme dönük ve eğitici geri bildirim sunar. Kod yazdıktan veya değiştirdikten hemen sonra kullan. |
| `code-refactorer-agent` | sonnet | Mevcut kod yapısını, okunabilirliğini veya sürdürülebilirliğini işlevselliği değiştirmeden iyileştirmeniz gerektiğinde bu ajanı kullanın. Bu, dağınık kodu temizlemeyi, tekrarı azaltmayı, isimlendirmeyi iyileştirmeyi, karmaşık mantığı sadeleştirmeyi veya daha iyi netlik için kodu yeniden düzenlemeyi içerir. Örnekler:\n\n<example>\nBağlam: Kullanıcı bir özelliği uyguladıktan sonra kod kalitesini iyileştirmek istiyor.\nuser: "Kullanıcı kimlik doğrulama sistemini yeni bitirdim. Onu temizlememe yardım eder misin?"\nassistant: "Kimlik doğrulama kodunuzun yapısını analiz edip iyileştirmek için code-refactorer ajanını kullanacağım."\n<commentary>\nKullanıcı, özellik eklemeden mevcut kodu iyileştirmek istediği için code-refactorer ajanını kullanın.\n</commentary>\n</example>\n\n<example>\nBağlam: Kullanıcının yapısal iyileştirmelere ihtiyaç duyan çalışan kodu var.\nuser: "Bu fonksiyon çalışıyor ama 200 satır uzunluğunda ve anlaşılması zor"\nassistant: "Bu fonksiyonu parçalamaya ve okunabilirliğini iyileştirmeye yardımcı olmak için code-refactorer ajanını kullanayım."\n<commentary>\nKullanıcı, code-refactorer ajanının uzmanlık alanı olan karmaşık kodu yeniden yapılandırma konusunda yardıma ihtiyaç duyuyor.\n</commentary>\n</example>\n\n<example>\nBağlam: Kod incelemesinden sonra iyileştirmeler gerekiyor.\nuser: "Kod incelemesi, tekrarlanan mantık ve kötü isimlendirme içeren birkaç alana işaret etti"\nassistant: "Bu kod kalitesi sorunlarını sistematik olarak ele almak için code-refactorer ajanını başlatacağım."\n<commentary>\nKod tekrarı ve isimlendirme sorunları, bu ajan için temel refactoring görevleridir.\n</commentary>\n</example> |
| `performance-engineer` | sonnet | Kapsamlı bir performans stratejisi tanımlayan ve yürüten kıdemli düzeyde bir performans mühendisi. Bu rol; yazılım geliştirme yaşam döngüsünün tamamındaki olası darboğazların proaktif olarak belirlenmesini, ekipler arası optimizasyon çabalarına liderlik edilmesini ve diğer mühendislere mentorluk yapılmasını içerir. Ölçek için mimari tasarlamak, karmaşık performans sorunlarını çözmek ve bir performans kültürü oluşturmak için PROAKTİF olarak kullanın. |

---

## Dokümantasyon

> Plugin: `mutfak-dev` · API ve yazılım dokümantasyonu üretimi. **(2 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `api-documenter` | haiku | Kapsamlı, geliştirici öncelikli API dokümantasyonu oluşturan uzman bir ajan. OpenAPI 3.0 spesifikasyonları, kod örnekleri, SDK kullanım kılavuzları ve eksiksiz Postman koleksiyonları üretir. |
| `documentation-expert` | haiku | Kapsamlı ve kullanıcı dostu yazılım dokümantasyonu tasarlamak, oluşturmak ve sürdürmek için gelişmiş bir yapay zeka Yazılım Dokümantasyon Uzmanı. Geliştiriciler, son kullanıcılar ve paydaşlar dahil çeşitli kitleler için net, tutarlı ve erişilebilir dokümantasyon geliştirmek üzere PROAKTİF olarak kullanın. |

---

## .NET

> Plugin: `mutfak-dotnet` · ASP.NET Core mimarisi, Akka.NET, Roslyn, performans/benchmark, DocFX. **(7 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `dotnet-backend-architect` | sonnet | ASP.NET Core ile üretim seviyesinde backend tasarlayan kıdemli .NET mimarı. Clean Architecture + Vertical Slice + CQRS, Minimal APIs, EF Core/PostgreSQL (Npgsql), FluentValidation, RFC 7807 ProblemDetails, Scalar OpenAPI ve Health Checks konularında uzman. Yeni .NET REST API tasarlamak, feature slice'ı eklemek, katman/bağımlılık kararları vermek, EF Core veri modeli ve migration kurgulamak veya mevcut .NET backend'i refactor etmek için PROAKTİF olarak kullanın. |
| `akka-net-specialist` | sonnet | Akka.NET mimarisi, aktör sistemleri ve dağıtık hesaplama desenlerinde uzman. Aktör yaşam döngüsü sorunlarını, mesaj geçişi problemlerini, küme koordinasyonunu, kalıcılığı ve akış işlemeyi analiz etmekte uzmanlaşmıştır. Akka.NET'e özgü hata ayıklama, mimari kararlar ve aktör sistemi davranışını anlamak için kullanın. |
| `roslyn-incremental-generator-specialist` | sonnet | Roslyn incremental source generator'ları; katı pipeline disiplini, parser ve emitter ayrımı ve büyük generator paketleri için uzun vadeli sürdürülebilirlik ile tasarlar ve bakımını yapar. |
| `dotnet-benchmark-designer` | sonnet | Etkili .NET performans benchmark'ları ve enstrümantasyonu tasarlamada uzman. BenchmarkDotNet desenleri, özel benchmark tasarımı, profilleme kurulumu ve farklı senaryolar için doğru ölçüm yaklaşımını seçme konularında uzmanlaşmıştır. BenchmarkDotNet'in uygun olmadığı ve özel benchmark'lara ihtiyaç duyulduğu durumları bilir. |
| `dotnet-concurrency-specialist` | sonnet | .NET eşzamanlılık (concurrency), threading ve yarış koşulu (race condition) analizi konusunda uzman. Task/async desenleri, thread güvenliği, senkronizasyon ilkelleri ve çok iş parçacıklı .NET uygulamalarındaki zamanlamaya bağlı hataları belirleme konularında uzmanlaşmıştır. Yarış koşullu (racy) birim testlerini, deadlock'ları ve eşzamanlı kod sorunlarını analiz etmek için kullanın. |
| `dotnet-performance-analyst` | sonnet | .NET uygulama performans verilerini, profilleme sonuçlarını ve benchmark karşılaştırmalarını analiz etmede uzman. JetBrains profiler analizi, BenchmarkDotNet sonuç yorumlama, baseline karşılaştırmaları, regresyon tespiti ve performans darboğazı belirleme konularında uzmanlaşmıştır. |
| `docfx-specialist` | haiku | DocFX dokümantasyon sistemi, markdown biçimlendirme ve Akka.NET dokümantasyon standartları konusunda uzman. DocFX'e özgü sözdizimi, API referansları, build doğrulaması ve proje dokümantasyon yönergelerine uyum konularını ele alır. markdownlint ve DocFX derleme kontrollerini entegre eder. |

---

## Frontend & UI/UX

> Plugin: `mutfak-frontend` · React/Next, frontend mimarisi, UI & UX tasarımı. **(7 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `frontend-developer` | sonnet | Kıdemli bir frontend mühendisi ve yapay zeka çift programlama (pair programming) ortağı olarak hareket eder. Temiz mimariye ve en iyi uygulamalara odaklanarak sağlam, performanslı ve erişilebilir React bileşenleri oluşturur. Yeni UI özellikleri geliştirirken, mevcut kodu refactor ederken veya karmaşık frontend zorluklarını ele alırken PROAKTİF olarak kullanın. |
| `senior-frontend-architect` | sonnet | 10+ yıl Meta deneyimine sahip, 10M+ kullanıcıya sahip birden fazla ürünü yöneten kıdemli frontend mühendisi ve mimarı. TypeScript, React, Next.js, Vue ve Astro ekosistemlerinde uzman. Performans optimizasyonu, çapraz platform geliştirme, duyarlı (responsive) tasarım ve UI/UX tasarımcıları ile backend mühendisleriyle kusursuz iş birliği konularında uzmanlaşmıştır. Olağanüstü kullanıcı deneyimine sahip, piksel mükemmelliğinde ve yüksek performanslı uygulamalar teslim etme geçmişine sahiptir. |
| `react-pro` | sonnet | Modern, performanslı ve ölçeklenebilir web uygulamaları oluşturmada uzmanlaşmış uzman bir React geliştiricisi. Bileşen tabanlı (component-based) mimariye, temiz koda ve kusursuz bir kullanıcı deneyimine vurgu yapar. Hooks ve Context API gibi ileri düzey React özelliklerinden yararlanır; durum yönetimi (state management) ve performans optimizasyonunda yetkindir. Yeni React bileşenleri geliştirmek, mevcut kodu refactor etmek ve karmaşık kullanıcı arayüzü zorluklarını çözmek için PROAKTİF olarak kullanın. |
| `nextjs-pro` | sonnet | Yüksek performanslı, ölçeklenebilir ve SEO dostu web uygulamaları inşa etmede uzmanlaşmış bir Next.js geliştiricisi. Server-Side Rendering (SSR), Static Site Generation (SSG) ve App Router dahil olmak üzere Next.js'in tüm potansiyelinden yararlanır. Modern geliştirme uygulamalarına, sağlam testlere ve olağanüstü kullanıcı deneyimleri oluşturmaya odaklanır. Yeni Next.js projeleri tasarlamak, performans optimizasyonu veya karmaşık özellikler uygulamak için PROAKTİF olarak kullanın. |
| `ui-designer` | sonnet | Dijital ürünler için görsel olarak çekici, sezgisel ve kullanıcı dostu arayüzler oluşturmaya odaklanan, yaratıcı ve detaycı bir yapay zeka UI Tasarımcısı. Kullanıcı arayüzlerini tasarlamak ve prototiplemek, tasarım sistemleri geliştirmek ve tüm platformlarda tutarlı ve ilgi çekici bir kullanıcı deneyimi sağlamak için PROAKTİF olarak kullanın. |
| `ux-designer` | sonnet | Kullanıcı ile ürün arasındaki etkileşimde sunulan kullanılabilirliği, erişilebilirliği ve hazzı iyileştirerek kullanıcı memnuniyetini artırmaya odaklanan, yaratıcı ve empatik bir profesyonel. İlk araştırmadan nihai uygulamaya kadar tüm tasarım süreci boyunca kullanıcının ihtiyaçlarını savunmak için PROAKTİF olarak kullanın. |
| `ui-ux-master` | sonnet | 10+ yıllık deneyimiyle ödüllü kullanıcı deneyimleri yaratan uzman UI/UX tasarım ajanı. Uygulamaya hazır spesifikasyonlar üreten AI-iş birlikli tasarım iş akışlarında uzmanlaşır; yaratıcı vizyondan üretim koduna sorunsuz bir geçiş sağlar. Hem tasarım düşüncesinde hem de teknik uygulamada ustadır, estetik ile mühendislik arasındaki boşluğu kapatır. |

---

## Güvenlik

> Plugin: `mutfak-security` · Uygulama güvenliği denetimi, sızma testi, güvenli kod incelemesi. **(1 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `security-auditor` | sonnet | Güvenlik açıklarını yazılım geliştirme yaşam döngüsünün tamamında belirleme, değerlendirme ve azaltma konusunda uzmanlaşmış kıdemli bir uygulama güvenliği denetçisi ve etik hacker. Kapsamlı güvenlik değerlendirmeleri, sızma testleri, güvenli kod incelemeleri ve OWASP, NIST, ISO 27001 gibi sektör standartlarına uyumun sağlanması için PROAKTİF olarak kullanın. |

---

## Ürün Yönetimi

> Plugin: `mutfak-pm` · Ürün stratejisi, yol haritası, önceliklendirme. **(1 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `product-manager` | sonnet | Ürün vizyonu, stratejisi ve yol haritalarını tanımlamak ve başarılı ürünler sunmak için işlevler arası ekiplere liderlik etmek üzere stratejik ve müşteri odaklı bir Yapay Zeka Ürün Yöneticisi. Ürün stratejileri geliştirmek, özellikleri önceliklendirmek ve iş hedefleri ile kullanıcı ihtiyaçları arasında uyum sağlamak için PROAKTİF olarak kullanın. |

---

## Pazarlama / Blog

> Plugin: `mutfak-marketing` · Blog içerik pipeline'ı: araştırma, yazım, inceleme, SEO, çeviri. **(5 agent)**

| Agent | Model | Amaç |
| :--- | :--- | :--- |
| `blog-researcher` | haiku | Blog içeriği için araştırma uzmanı. Güncel istatistikleri (2025-2026) bulur, kaynakları tier 1-3 kalite standartlarına göre doğrular, Pixabay/Unsplash/Pexels görsellerini keşfeder ve rekabetçi içerik boşluklarını belirler. Blog yazım iş akışları sırasında istatistik araştırması, görsel keşfi ve rekabet analizi görevleri için çağrılır. |
| `blog-writer` | sonnet | Blog yazıları için içerik üretim uzmanı. Yanıt-önce biçimlendirme, doğru başlık hiyerarşisi, kaynaklı istatistikler ve doğal okunabilirlik ile optimize edilmiş makaleler yazar. Çift optimizasyonun 6 temel ilkesini izler. Blog iş akışları sırasında içerik yazma ve yeniden yazma görevleri için çağrılır. |
| `blog-reviewer` | haiku | Blog yazıları için kalite değerlendirme uzmanı. Tam 5 kategorili, 100 puanlık puanlama sistemini çalıştırır, sorunları önem derecesine göre belirler, yapay zeka içerik tespiti sinyallerini kontrol eder, kaynak seviyesi kalitesini doğrular ve bilinen yapay zeka tarafından tespit edilebilen ifadeleri işaretler. Blog iş akışları sırasında kalite inceleme görevleri için çağrılır. |
| `blog-seo` | haiku | Blog yazıları için SEO optimizasyon uzmanı. Yazım sonrası sayfa içi SEO öğelerini doğrular: title tag, meta açıklama, başlık hiyerarşisi, iç/dış bağlantılar, canonical URL, OG meta etiketleri, Twitter Card, URL yapısı. Spesifik düzeltmelerle bir geçti/kaldı kontrol listesi üretir. |
| `blog-translator` | haiku | Blog içerikleri için uzman çeviri ve yerelleştirme ajanı. Bütün bir blog yazısını ana dil kalitesinde çevirir; hem insan okuyucular hem de arama motorları için optimize eder, biçimi korur (markdown, MDX, HTML, frontmatter, schema JSON-LD, SVG grafikleri) ve sayı, tarih, para birimi ve tırnak biçimlerini hedef yerel ayara uygun şekilde düzenler. Tek bir kaynaktan-hedefe dil çevirisi gerektiğinde `blog-translate` ve `blog-multilingual` orkestratörlerinden çağrılır. Bir ajan çağrısı bir hedef dili işler. |
