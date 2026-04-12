try {
    use("DeviceManagementDB")



    //Password for alexandruiacob01@gmail.com -> 12345678 , oananagy@yahoo.com -> paroladetest

    if (db.Users.countDocuments() === 0) {
        db.Users.insertMany([
            { name: "Alexandru Iacob", role: "Fullstack Developer", location: "Cluj-Napoca", email: "alexandruiacob01@gmail.com", passwordHash: "$2a$11$HTEUGGX4Ck71epk8sgmAfOoSuxJ5/6N7YewHk9nTP/RTIqGciZ74O" },
            { name: "Oana Nagy", role: "Senior Backend Developer", location: "Timisoara", email: "oananagy@yahoo.com", passwordHash: "$2a$11$A0Wya3JVnnnFnqgLh5PkW.3iVEG3h.Sy0dhAy0pX1kTawEl9d1gQS" },
            { name: "Andrei Ilie", role: "QA Automation", location: "Oradea", email: "andreiclaudiu@outlook.com", passwordHash: "$2a$11$vEczzzx6kpwATqBtRmPNFOI.2dNoEwgl6Ce7TuX1gDf5nG63Tvd8i" }
        ]);
    }


    if (db.Devices.countDocuments() === 0) {
        db.Devices.insertMany([
            { name: "iPhone 15 Pro", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "17.0", processor: "A17 Pro", ram: 8, description: "A premium Apple smartphone with a titanium design and advanced camera system.", assignedUserId: null },
            { name: "iPhone 15", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "17.0", processor: "A16 Bionic", ram: 6, description: "A powerful Apple smartphone with an improved camera system and Dynamic Island.", assignedUserId: null },
            { name: "iPhone 14 Pro Max", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "16.0", processor: "A16 Bionic", ram: 6, description: "Apple's largest Pro smartphone with a stunning 6.7-inch display and ProMotion technology.", assignedUserId: null },
            { name: "iPhone 14", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "16.0", processor: "A15 Bionic", ram: 6, description: "A reliable Apple smartphone with crash detection and emergency SOS via satellite.", assignedUserId: null },
            { name: "iPhone 13 Pro", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "15.0", processor: "A15 Bionic", ram: 6, description: "A professional Apple smartphone with a ProMotion display and macro photography support.", assignedUserId: null },
            { name: "iPhone SE 3rd Gen", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "15.0", processor: "A15 Bionic", ram: 4, description: "Apple's most affordable smartphone with powerful performance in a compact design.", assignedUserId: null },
            { name: "iPad Pro 12.9", manufacturer: "Apple", type: "tablet", os: "iPadOS", osVersion: "17.0", processor: "M2", ram: 16, description: "A powerful Apple tablet designed for professional use with a large Liquid Retina display.", assignedUserId: null },
            { name: "iPad Air 5", manufacturer: "Apple", type: "tablet", os: "iPadOS", osVersion: "16.0", processor: "M1", ram: 8, description: "A versatile Apple tablet with M1 chip power in a lightweight and colorful design.", assignedUserId: null },
            { name: "Galaxy S24", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "14.0", processor: "Snapdragon 8 Gen 3", ram: 12, description: "A flagship Samsung smartphone with AI-powered features and a bright AMOLED display.", assignedUserId: null },
            { name: "Galaxy S24 Ultra", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "14.0", processor: "Snapdragon 8 Gen 3", ram: 12, description: "Samsung's most powerful smartphone with built-in S Pen and 200MP camera system.", assignedUserId: null },
            { name: "Galaxy S23", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "13.0", processor: "Snapdragon 8 Gen 2", ram: 8, description: "A compact Samsung flagship with excellent camera performance and long battery life.", assignedUserId: null },
            { name: "Galaxy S23 FE", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "13.0", processor: "Snapdragon 8 Gen 1", ram: 8, description: "Samsung's fan edition flagship offering premium features at a more accessible price.", assignedUserId: null },
            { name: "Galaxy A54", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "13.0", processor: "Exynos 1380", ram: 8, description: "A mid-range Samsung smartphone with a stunning display and versatile camera system.", assignedUserId: null },
            { name: "Galaxy Tab S9", manufacturer: "Samsung", type: "tablet", os: "Android", osVersion: "13.0", processor: "Snapdragon 8 Gen 2", ram: 12, description: "Samsung's premium Android tablet with an AMOLED display and S Pen support.", assignedUserId: null },
            { name: "Galaxy Tab S9 FE", manufacturer: "Samsung", type: "tablet", os: "Android", osVersion: "13.0", processor: "Exynos 1380", ram: 6, description: "An affordable Samsung tablet with a large display ideal for entertainment and productivity.", assignedUserId: null },
            { name: "Pixel 8", manufacturer: "Google", type: "phone", os: "Android", osVersion: "14.0", processor: "Tensor G3", ram: 8, description: "A Google smartphone focused on computational photography and clean Android experience.", assignedUserId: null },
            { name: "Pixel 8 Pro", manufacturer: "Google", type: "phone", os: "Android", osVersion: "14.0", processor: "Tensor G3", ram: 12, description: "Google's premium smartphone with advanced AI features and a pro-grade camera system.", assignedUserId: null },
            { name: "Surface Pro 9", manufacturer: "Microsoft", type: "tablet", os: "Windows", osVersion: "11", processor: "Intel Core i5", ram: 16, description: "A versatile Microsoft tablet that doubles as a laptop, ideal for business productivity.", assignedUserId: null },
            { name: "OnePlus 12", manufacturer: "OnePlus", type: "phone", os: "Android", osVersion: "14.0", processor: "Snapdragon 8 Gen 3", ram: 12, description: "A flagship OnePlus smartphone with Hasselblad camera tuning and ultra-fast charging.", assignedUserId: null },
            { name: "Xiaomi 14 Pro", manufacturer: "Xiaomi", type: "phone", os: "Android", osVersion: "14.0", processor: "Snapdragon 8 Gen 3", ram: 12, description: "A premium Xiaomi smartphone with Leica optics and a stunning curved AMOLED display.", assignedUserId: null }
        ]);
    }


} catch (err) {
    print("Error inserting data into collections",err)
}