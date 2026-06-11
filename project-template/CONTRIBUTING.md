# Katkıda Bulunma Rehberi

Mutfak Yazılımevi projelerine katkıda bulunduğunuz için teşekkürler! 🎉

## 🌿 Branch Stratejisi

- `main` — kararlı, üretime hazır kod
- `develop` — geliştirme dalı (varsa)
- `feature/<özellik-adı>` — yeni özellikler
- `fix/<hata-adı>` — hata düzeltmeleri
- `hotfix/<acil-düzeltme>` — acil üretim düzeltmeleri

## 📝 Commit Mesajları

[Conventional Commits](https://www.conventionalcommits.org) standardını kullanıyoruz:

```
<tip>(<kapsam>): <açıklama>

feat(auth): kullanıcı girişi eklendi
fix(api): null referans hatası düzeltildi
docs(readme): kurulum adımları güncellendi
```

Tipler: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

## 🔁 Pull Request Süreci

1. Depoyu fork'layın veya yeni bir branch açın.
2. Değişikliklerinizi yapın ve testlerin geçtiğinden emin olun.
3. Açıklayıcı bir başlıkla PR açın ve şablonu doldurun.
4. En az bir reviewer onayı bekleyin.
5. CI kontrolleri yeşil olmalıdır.

## ✅ Kod Standartları

- `.editorconfig` kurallarına uyun.
- Lint ve format kontrollerini commit öncesi çalıştırın.
- Yeni özellikler için test yazın.

## 🐛 Hata Bildirimi

Hataları [Issues](../../issues) üzerinden, ilgili şablonu kullanarak bildirin.
