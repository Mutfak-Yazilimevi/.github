# Autonomy — Otonom Çalışma

> Bu proje **otonom** yürür: Claude gereksiz soru sormaz. Makul varsayımlar yapar, bunları
> belirtir ve ilerler. Soru son çaredir — akışı bölmek için değil, gerçekten gerektiğinde.

## İlke

- **Önce harekete geç.** Bilgi yeterliyse uygula; her küçük kararı sormadan ilerle.
- **Varsay + belirt + devam et.** Belirsizlikte en makul seçeneği seç, "şunu varsaydım" diye
  kısaca not düş, geri dönülebilir bir işse devam et. Yanlışsa kullanıcı düzeltir.
- **Soruları topla.** Kaçınılmazsa tek tek değil, **işin sonunda toplu** sor.
- **Katalog-öncelikli:** soru sormadan önce `skills/agents-catalog`'u tara — cevap çoğu zaman
  bir skill/agent'tadır (`catalog.md`). "Bilmiyorum, sorayım" değil, "önce arayayım".

## Ne zaman SORULUR (yalnız bunlar)

- **Geri dönülemez / yıkıcı** işlem: veri/dosya silme, prod deploy, force push, şema göçü,
  toplu yeniden adlandırma.
- **Yüksek belirsizlik + yüksek bedel**: yanlış seçim pahalı **ve** makul bir varsayımla
  kapanmıyor (ör. temel ürün/iş yönü, mimari kilitlenme).
- **Dışa açık / kalıcı yayın**: publish, e-posta gönderme, üçüncü tarafa veri, kamuya açık paylaşım.
- Kullanıcının **açıkça** karar vermesi gereken ürün/iş/öncelik tercihi.

## Ne zaman SORULMAZ

- Hangi dosya/desen/isim/klasör — koddan ve konvansiyondan çıkar.
- Mekanik/teknik tercihler (format, yardımcı kütüphane, test düzeni) — varsay, ilerle.
- "Devam edeyim mi?" / "Şunu yapayım mı?" türü teyitler — **evet, yap.**
- Bilgi eksikse — önce **oku/araştır/katalog tara**, sonra hâlâ kritikse sor.

## Süreç kapılarıyla ilişki

`process.md` PRD onayı ve `/mutfak-onboard` "direktif bekle" kapıları **korunur** — bunlar
otonomiyi kısıtlamaz, **yön/güvenlik** kapısıdır (yanlış yöne otonom koşmayı önler). Onların
dışında varsayılan davranış: **sor değil, yap.**

## İzinlerle ilişki (`settings.json`)

- `defaultMode: "acceptEdits"` + geniş `permissions.allow` → dosya düzenleme, test, build, lint,
  git (push hariç) **otomatik** geçer; izin istemi çıkmaz.
- Yıkıcı/dışa-açık işler `ask`/`deny`'de tutulur (push, force-push, rm -rf, sudo, .env okuma).
- Daha da otonom: oturumda **Shift+Tab** ile `acceptEdits` moduna geç, veya
  `claude --permission-mode acceptEdits` ile başlat. Tam otonom (riskli): `--dangerously-skip-permissions`.

> Kısaca: **harekete meyilli ol; soruyu yıkıcı / belirsiz-ve-pahalı / dışa-açık durumlara sakla.**
