// ui_controller.js

async function mainOrchestrator(section) {
    if (section === 'barns') {
        const [barns, animals] = await Promise.all([
            FarmApi.getSectionData('barns'),
            FarmApi.getSectionData('animals')
        ]);
        if (!barns || !animals) return;
        window.currentBarnsData = barns.map(b => ({
            name: b.name || b.Name,
            spec: b.specialization || b.Specialization || 'إنتاج',
            cap: b.capacity || b.Capacity || 0,
            count: animals.filter(a => (a.barnName === b.name || a.barnId === b.id)).length
        }));
        renderBarns(window.currentBarnsData);
    }
    else if (section === 'animals') {
        const animals = await FarmApi.getSectionData('animals');
        if (animals) {
            window.currentAnimalsData = animals;
            renderAnimals(animals);
        }
    }
    else if (section === 'hatchery') {
        const hatchery = await FarmApi.getSectionData('hatchery');
        if (hatchery) {
            window.currentHatcheryData = hatchery;
            renderHatchery(hatchery);
        }
    }
}

function renderBarns(data) {
    document.getElementById('dynamic-filters').innerHTML = `
        <button onclick="sortBarns('desc')" class="btn btn-sm btn-primary">الأكثر عدداً ↓</button>
        <button onclick="sortBarns('asc')" class="btn btn-sm btn-secondary">الأقل عدداً ↑</button>
        <button onclick="sortBarns('az')" class="btn btn-sm btn-outline-dark">أبجدي (أ-ي)</button>
        <button onclick="sortBarns('za')" class="btn btn-sm btn-outline-dark">أبجدي (ي-أ)</button>`;

    document.getElementById('table-head').innerHTML = `
        <tr><th>اسم الحظيرة</th><th>التخصص</th><th>السعة الكلية</th><th>عدد الحيوانات</th></tr>`;

    document.getElementById('table-body').innerHTML = data.map(b => `
        <tr><td>${b.name}</td><td>${b.spec}</td><td>${b.cap}</td><td class="fw-bold text-success">${b.count}</td></tr>
    `).join('');
}

function sortBarns(type) {
    let data = window.currentBarnsData;
    if (!data) return;
    const logic = {
        'desc': (a, b) => b.count - a.count,
        'asc': (a, b) => a.count - b.count,
        'az': (a, b) => a.name.localeCompare(b.name),
        'za': (a, b) => b.name.localeCompare(a.name)
    };
    data.sort(logic[type]);
    renderBarns(data);
}

// أضف دوال renderAnimals و filterBySpecies هنا لاحقاً

// 1. الدالة الرئيسية للعرض (تحتوي على الأزرار الـ 4 والجدول)
function renderAnimals(data) {
    const filters = document.getElementById('dynamic-filters');
    const head = document.getElementById('table-head');
    const body = document.getElementById('table-body');

    // إنشاء الأزرار الأربعة المطلوبة + زر إعادة الضبط
    filters.innerHTML = `
        <div class="d-flex align-items-center gap-2 mb-3">
            <button onclick="sortAnimals('old')" class="btn btn-sm btn-dark">الأكبر عمراً ↑</button>
            <button onclick="sortAnimals('young')" class="btn btn-sm btn-dark">الأصغر عمراً ↓</button>
            <div class="vr mx-2" style="height: 20px;"></div>
            <button onclick="filterByGender('ذكر')" class="btn btn-sm btn-outline-primary">ذكور ♂</button>
            <button onclick="filterByGender('أنثى')" class="btn btn-sm btn-outline-danger">إناث ♀</button>
            <button onclick="mainOrchestrator('animals')" class="btn btn-sm btn-link text-muted">تحديث ↺</button>
        </div>
    `;

    // رأس الجدول (6 أعمدة مرتبة)
    head.innerHTML = `
        <tr>
            <th>رقم الحيوان</th>
            <th>الحيوان</th>
            <th>الفصيلة</th>
            <th>الجنس</th>
            <th>العمر (شهر)</th>
            <th>الحظيرة</th>
        </tr>`;

    // بناء الصفوف
    body.innerHTML = data.map(a => {
        // تنظيف بيانات الجنس لضمان عمل الألوان
        const genderRaw = (a.gender || a.Gender || "غير محدد").toString().trim();
        const isMale = (genderRaw.toLowerCase() === 'ذكر' || genderRaw.toLowerCase() === 'male');
        const genderClass = isMale ? 'badge bg-primary' : (genderRaw === 'غير محدد' ? 'badge bg-secondary' : 'badge bg-danger');

        return `
        <tr>
            <td>ID-${a.id}</td>
            <td class="fw-bold">${a.name || a.Name}</td>
            <td><span class="badge bg-light text-dark border">${a.species || a.Species}</span></td>
            <td><span class="${genderClass}">${genderRaw}</span></td>
            <td>${a.age || a.Age || 0}</td>
            <td>${a.barnName || a.BarnName || "-"}</td>
        </tr>`;
    }).join('');
}

// 2. دالة فلترة الجنس (ذكور / إناث) - تدعم كل أشكال الكتابة
function filterByGender(type) {
    const allData = window.currentAnimalsData;
    if (!allData) return;

    const filtered = allData.filter(a => {
        let g = (a.gender || a.Gender || "").toString().trim().toLowerCase();
        if (type === 'ذكر') return g === 'ذكر' || g === 'male';
        if (type === 'أنثى') return g === 'أنثى' || g === 'انثى' || g === 'female';
        return false;
    });

    renderAnimals(filtered);
}

// 3. دالة ترتيب العمر (أكبر / أصغر)
function sortHatchery(order) {
    // التأكد من وجود بيانات مخزنة
    if (!window.currentHatcheryData) return;

    let data = [...window.currentHatcheryData];

    data.sort((a, b) => {
        // نستخدم المسمى الذي حددناه في الـ Select داخل الـ Controller
        let dateA = new Date(a.productionDate || a.ProductionDate);
        let dateB = new Date(b.productionDate || b.ProductionDate);

        // إذا كان التاريخ غير صحيح نضعه في الأخير
        if (isNaN(dateA)) return 1;
        if (isNaN(dateB)) return -1;

        return order === 'near' ? dateA - dateB : dateB - dateA;
    });

    renderHatchery(data);
}

function renderHatchery(data) {
    const filters = document.getElementById('dynamic-filters');
    const head = document.getElementById('table-head');
    const body = document.getElementById('table-body');

    filters.innerHTML = `
        <div class="d-flex align-items-center gap-2 mb-3">
            <button onclick="sortHatchery('near')" class="btn btn-sm btn-warning fw-bold">📅 الأقرب إنتاجاً</button>
            <button onclick="sortHatchery('far')" class="btn btn-sm btn-outline-warning text-dark">⏳ الأبعد موعداً</button>
            <button onclick="mainOrchestrator('hatchery')" class="btn btn-sm btn-link text-muted ms-auto">تحديث ↺</button>
        </div>
    `;

    head.innerHTML = `
        <tr>
            <th>رقم العملية</th>
            <th>الفصيلة</th>
            <th>الآباء (ذكر × أنثى)</th>
            <th>موعد الإنتاج المتوقع</th>
            <th>الحالة</th>
            <th>حظيرة الإنتاج</th>
        </tr>`;

    if (!data || data.length === 0) {
        body.innerHTML = `<tr><td colspan="6" class="text-center p-4 text-muted">لا توجد عمليات تفريخ جارية حالياً</td></tr>`;
        return;
    }

    body.innerHTML = data.map(h => {
        const dateObj = new Date(h.productionDate || h.ProductionDate);
        const formattedDate = isNaN(dateObj) ? "غير محدد" : dateObj.toLocaleDateString('ar-EG');

        return `
        <tr>
            <td>Batch-${h.id || h.Id}</td>
            <td class="fw-bold">${h.species || h.Species}</td>
            <td><small class="text-muted">${h.parentInfo || h.ParentInfo || "-"}</small></td>
            <td class="text-primary fw-bold">${formattedDate}</td>
            <td><span class="badge bg-info text-dark">${h.status || h.Status}</span></td>
            <td>${h.barnName || h.BarnName}</td>
        </tr>`;
    }).join('');
}