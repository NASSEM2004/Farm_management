// farm_filters.js - وظيفة هذا الملف هي التواصل مع الـ API فقط
const FarmApi = {
    async getSectionData(sectionName) {
        try {
            const response = await fetch(`/api/FarmData/${sectionName}`);
            if (!response.ok) throw new Error("Network response was not ok");
            return await response.json();
        } catch (error) {
            console.error("حدث خطأ أثناء جلب البيانات:", error);
            return null;
        }
    }
};