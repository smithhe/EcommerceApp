import {useState, useEffect} from 'react';
import {useNavigate} from 'react-router-dom';
import {toast, ToastContainer} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {Category} from "../models/Category.ts";
import categoryService from "../services/CategoryService.ts";

const Categories = () => {
    const [categoryList, setCategoryList] = useState<Category[]>([]);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchCategories = async () => {
            const response = await categoryService.getAllCategories();
            if (response.success) {
                setCategoryList(response.categories);
            } else {
                toast.error(response.message);
            }
        };

        fetchCategories();
    });

    const onCategoryButtonClick = (categoryId: number) => {
        navigate(`/products/${categoryId}`);
    };

    if (!categoryList) {
        return <p><em>Loading...</em></p>;
    }

    return (
        <div className="container mt-5">
            <div className="header text-center">
                <h1 className="category-heading">Explore Our Categories</h1>
            </div>
            <div className="row mt-3">
                {categoryList.map((category, index) => (
                    <div className="col-md-4 mb-4" key={index}>
                        <div className="card">
                            <div className="card-body">
                                <h5 className="card-title">{category.name}</h5>
                                <p className="card-text">{category.summary}</p>
                                <a className="btn btn-primary" onClick={() => onCategoryButtonClick(category.id)}>View{category.name} </a>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            <ToastContainer/>
        </div>
    );
};

export default Categories;